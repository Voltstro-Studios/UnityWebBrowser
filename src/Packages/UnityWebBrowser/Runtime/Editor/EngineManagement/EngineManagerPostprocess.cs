// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Editor.EngineManagement
{
    internal class EngineManagerPostprocess : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result is BuildResult.Failed or BuildResult.Cancelled)
                return;

            BuildTarget buildTarget = report.summary.platform;
            Platform buildPlatform = buildTarget.UnityBuildTargetToPlatform();

            string buildFullOutputPath = report.summary.outputPath;
            string buildAppName = Path.GetFileNameWithoutExtension(buildFullOutputPath);
            string buildOutputPath = Path.GetDirectoryName(buildFullOutputPath);

            Debug.Log("Copying engine process files...");

            List<Engine> engines = EditorHelper.FindAssetsByType<Engine>();

            if (engines.Count == 0)
            {
                Debug.LogWarning("There were no UWB engines found to copy!");
                return;
            }

            //We need to get the build's data folder
            string buildDataPath = Path.GetFullPath($"{buildOutputPath}/{buildAppName}_Data/");
            if (buildTarget == BuildTarget.StandaloneOSX)
                buildDataPath =
                    Path.GetFullPath($"{buildOutputPath}/{buildAppName}.app/Contents/Resources/Data/");

            //Make sure the data folder exists
            if (!Directory.Exists(buildDataPath))
            {
                Debug.LogError(
                    "Failed to get the build's data folder! Make sure your build is the same name as your product name (In your project settings).");
                return;
            }

            //UWB folder in the data folder
            string buildUwbPath = $"{buildDataPath}/UWB/";

            //Make sure it exists
            DirectoryInfo buildUwbInfo = new(buildUwbPath);
            if (!buildUwbInfo.Exists)
            {
                Directory.CreateDirectory(buildUwbPath);
            }
            else //If the directory exists, clear it
            {
                foreach (FileInfo fileInfo in buildUwbInfo.EnumerateFiles())
                    fileInfo.Delete();

                foreach (DirectoryInfo directoryInfo in buildUwbInfo.EnumerateDirectories())
                    directoryInfo.Delete(true);
            }

            buildUwbPath = Path.GetFullPath(buildUwbPath);

            foreach (Engine engine in engines)
                if (engine.EngineFiles.Any(x => x.platform == buildPlatform))
                {
                    Debug.Log("Copying UWB engine files...");

                    Engine.EnginePlatformFiles engineFiles =
                        engine.EngineFiles.First(x => x.platform == buildPlatform);

                    //Get the location where we are copying all the files
                    string engineFilesDir = Path.GetFullPath(engineFiles.engineFileLocation);
                    if (!Directory.Exists(engineFilesDir))
                    {
                        Debug.LogError("The engine files directory doesn't exist!");
                        continue;
                    }

                    string engineFilesParentDir = Directory.GetParent(engineFilesDir)?.Name;

                    //Get all files that aren't Unity .meta files
                    //NOTE: UWB 2.0 stores it's engine files in '~' folder, which is excluded by Unity, so we shouldn't need this anymore, but we will keep it anyway
                    string[] files = Directory.EnumerateFiles(engineFilesDir, "*.*", SearchOption.AllDirectories)
                        .Where(fileType => !fileType.EndsWith(".meta"))
                        .ToArray();

                    int size = files.Length;
                    Debug.Log($"Found {size} number of files to copy...");

                    //Now to copy all the files.
                    //We need to keep the structure of the process
                    for (int i = 0; i < size; i++)
                    {
                        string file = files[i];
                        string destFileName = Path.GetFileName(file);
                        EditorUtility.DisplayProgressBar("Copying UWB Engine Files",
                            $"Copying {destFileName}", i / size);

                        //If the file is not at the parent directory, then we need to create the directory in the UWB folder
                        string parentDirectory = "";
                        if (Directory.GetParent(file)?.Name != engineFilesParentDir)
                        {
                            parentDirectory = $"{Directory.GetParent(file)?.Name}/";

                            if (!Directory.Exists($"{buildUwbPath}{parentDirectory}"))
                                Directory.CreateDirectory($"{buildUwbPath}{parentDirectory}");
                        }

                        //Copy the file
                        File.Copy(file, $"{buildUwbPath}{parentDirectory}{destFileName}", true);
                    }

                    EditorUtility.ClearProgressBar();
                }

            Debug.Log("Done!");
        }
    }
}

#endif