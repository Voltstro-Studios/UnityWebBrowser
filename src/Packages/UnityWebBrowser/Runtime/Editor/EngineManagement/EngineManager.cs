#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityWebBrowser.Core.Engines;
using UnityWebBrowser.Shared.Core;

namespace UnityWebBrowser.Editor.EngineManagement
{
    public static class EngineManager
    {
        public static string GetEngineDirectory(Engine engine, Platform platform)
        {
            Engine.EnginePlatformFiles files = engine.EngineFiles.FirstOrDefault(x => x.platform == platform);
            if (files.engineFileLocation == null)
            {
                Debug.LogError(engine.EngineFilesNotFoundError);
                return null;
            }

            return Path.GetFullPath(files.engineFileLocation);
        }

        public static string GetEngineDirectory(Engine engine) =>
            GetEngineDirectory(engine, GetCurrentEditorPlatform());
        
        public static string GetEngineProcessFullPath(Engine engine, Platform platform)
        {
            string appPath = $"{GetEngineDirectory(engine, platform)}{engine.GetEngineExecutableName()}";
            if (platform == Platform.Windows64)
                appPath += ".exe";
            
            return Path.GetFullPath(appPath);
        }
        
        public static string GetEngineProcessFullPath(Engine engine) => GetEngineProcessFullPath(engine, GetCurrentEditorPlatform());

        private static Platform GetCurrentEditorPlatform()
        {
#if UNITY_EDITOR_LINUX
            Platform platform = Platform.Linux64;
#elif UNITY_EDITOR_WIN
            Platform platform = Platform.Windows64;
#elif UNITY_EDITOR_OSX
            Platform platform = Platform.MacOS;
#else
#error Unsupported platform!
            
#endif

            return platform;
        }
        
        [PostProcessBuild(1)]
        private static void CopyFilesOnBuild(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log("Copying browser engine files...");

            List<Engine> engines = EditorHelper.FindAssetsByType<Engine>();

            if (engines.Count == 0)
            {
                Debug.LogWarning("There were no engines found to copy!");
                return;
            }

            //Get full dir
            pathToBuiltProject = Path.GetDirectoryName(pathToBuiltProject);

            //We need to get the built project's plugins folder
            string buildPluginsDir = Path.GetFullPath($"{pathToBuiltProject}/{Application.productName}_Data/UWB/");
            if (target == BuildTarget.StandaloneOSX)
            {
                buildPluginsDir =
                    Path.GetFullPath($"{pathToBuiltProject}/{Application.productName}.app/Contents/Resources/Data/UWB/");
            }

            //Make sure it exists
            if (!Directory.Exists(buildPluginsDir))
                Directory.CreateDirectory(buildPluginsDir);

            foreach (Engine engine in engines)
            {
                if (engine.EngineFiles.Any(x => x.platform == GetCurrentEditorPlatform()))
                {
                    Debug.Log("Copying UWB engine files...");
                    
                    Engine.EnginePlatformFiles engineFiles =
                        engine.EngineFiles.First(x => x.platform == GetCurrentEditorPlatform());

                    //Get the location where we are copying all the files
                    string engineFilesDir = Path.GetFullPath(engineFiles.engineFileLocation);
                    if (!Directory.Exists(engineFilesDir))
                    {
                        Debug.LogError("The engine files directory doesn't exist!");
                        continue;
                    }
                    
                    string engineFilesParentDir = Directory.GetParent(engineFilesDir)?.Name;
                    
                    //Get all files that aren't Unity .meta files
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

                        //If the file is not at the parent directory, then we need to create the directory in the build
                        string parentDirectory = "";
                        if (Directory.GetParent(file)?.Name != engineFilesParentDir)
                        {
                            parentDirectory = $"{Directory.GetParent(file)?.Name}/";

                            if (!Directory.Exists($"{buildPluginsDir}{parentDirectory}"))
                                Directory.CreateDirectory($"{buildPluginsDir}{parentDirectory}");
                        }

                        //Copy the file
                        File.Copy(file, $"{buildPluginsDir}{parentDirectory}{destFileName}", true);
                    }

                    EditorUtility.ClearProgressBar();
                }
            }

            Debug.Log("Done!");
        }
    }
}

#endif