// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#if UNITY_EDITOR && !UWB_DISABLE_POSTPROCESSOR

using System;
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

#if UWB_ENGINE_PRJ //For CI reasons
            if(Application.isBatchMode)
                return;
#endif

            BuildTarget buildTarget = report.summary.platform;
            Platform buildPlatform;
            try
            {
                buildPlatform = buildTarget.UnityBuildTargetToPlatform();
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.LogWarning("UWB engine will not be copied! Unsupported platform.");
                return;
            }

            string buildFullOutputPath = report.summary.outputPath;
            string buildAppName = Path.GetFileNameWithoutExtension(buildFullOutputPath);
            string buildOutputPath = Path.GetDirectoryName(buildFullOutputPath)!;
            
            List<Engine> engines = EditorHelper.FindAssetsByType<Engine>();

            if (engines.Count == 0)
            {
                Debug.LogWarning("There were no UWB engines found to copy!");
                return;
            }

            string builtAppFolder = Path.GetFullPath(buildTarget == BuildTarget.StandaloneOSX
                ? Path.Combine(buildOutputPath, $"{buildAppName}.app", "Contents/Frameworks/")
                : buildOutputPath);
            
            //Make sure the build folder exists
            if (!Directory.Exists(builtAppFolder))
            {
                Debug.LogError(
                    "Failed to get the build's folder! Make sure your build is the same name as your product name (In your project settings).");
                return;
            }

            //We need to get folder where UWB will live
            string buildDataPath =
                Path.GetFullPath(
                    buildTarget == BuildTarget.StandaloneOSX 
                        ? Path.Combine(buildOutputPath, $"{buildAppName}.app", "Contents/Frameworks/") 
                        : Path.Combine(buildOutputPath, $"{buildAppName}_Data/UWB/"));

            //MacOS has a more customized way of deleting
            if (buildTarget != BuildTarget.StandaloneOSX)
            {
                if (Directory.Exists(buildDataPath))
                    Directory.Delete(buildDataPath, true);
            }

            foreach (Engine engine in engines)
                if (engine.EngineFiles.Any(x => x.platform == buildPlatform))
                {
                    Debug.Log("Copying UWB engine files...");

                    Engine.EnginePlatformFiles engineFiles =
                        engine.EngineFiles.First(x => x.platform == buildPlatform);

                    //Get the location where we are copying all the files
                    string engineFilesDir = Path.GetFullPath(engineFiles.engineEditorLocation);
                    if (!Directory.Exists(engineFilesDir))
                    {
                        Debug.LogError("The engine files directory doesn't exist!");
                        continue;
                    }

                    //Delete app path first on MacOS (if it exists)
                    string engineBuildPath = buildDataPath;
                    if (buildTarget == BuildTarget.StandaloneOSX)
                    {
                        engineBuildPath = Path.Combine(buildDataPath, $"{engine.GetEngineExecutableName()}.app");
                        if(Directory.Exists(engineBuildPath))
                            Directory.Delete(engineBuildPath, true);
                        
                        engineFilesDir = Path.Combine(engineFilesDir, $"{engine.GetEngineExecutableName()}.app");
                    }
                    
                    Debug.Log($"Copying engine files from {engineFilesDir} to {buildDataPath}...");
                    EditorUtility.DisplayProgressBar("Copying UWB Engine Files", $"Copying engine files from {engineFilesDir} to {buildDataPath}...", 0);
                    
                    FileUtil.CopyFileOrDirectory(engineFilesDir, engineBuildPath);
                    
                    EditorUtility.ClearProgressBar();
                }

            Debug.Log("Done!");
        }
    }
}

#endif