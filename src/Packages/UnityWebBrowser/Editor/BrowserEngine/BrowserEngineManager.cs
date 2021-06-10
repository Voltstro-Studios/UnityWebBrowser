using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityWebBrowser.Editor
{
    public static class BrowserEngineManager
    {
        private static readonly List<BrowserEngine> BrowserEngines = new List<BrowserEngine>();

        public static List<BrowserEngine> Engines => BrowserEngines;

        public static void AddBrowserEngine(BrowserEngine engine)
        {
            if (!CheckIfAppExists(engine))
            {
                Debug.LogError($"{engine.EngineName} is setup incorrectly!");
                return;
            }
            
            BrowserEngines.Add(engine);
        }

        public static BrowserEngine GetBrowser(string engineAppName)
        {
            return BrowserEngines.FirstOrDefault(x => x.EngineAppFile == engineAppName);
        }

        private static bool CheckIfAppExists(BrowserEngine engine)
        {
            foreach (KeyValuePair<BuildTarget,string> files in engine.BuildFiles)
            {
                string path = Path.GetFullPath(files.Value);
                string engineFile;
                if (files.Key == BuildTarget.StandaloneWindows || files.Key == BuildTarget.StandaloneWindows64)
                    engineFile = $"{engine.EngineAppFile}.exe";
                else
                    engineFile = engine.EngineAppFile;

                if (!File.Exists($"{path}{engineFile}"))
                {
                    Debug.LogError($"{files.Key} target is missing {engine.EngineAppFile}!");
                    return false;
                }
            }
            
            return true;
        }

        [PostProcessBuild(1)]
        public static void CopyFilesOnBuild(BuildTarget target, string pathToBuiltProject)
        {
            Debug.Log("Copying browser engine files...");

            if (BrowserEngines.Count == 0)
            {
                Debug.LogWarning("No browser engines to copy!");
                return;
            }
            
            //Get full dir
            pathToBuiltProject = Path.GetDirectoryName(pathToBuiltProject);
            
            //We need to get the built project's plugins folder
            string buildPluginsDir = Path.GetFullPath($"{pathToBuiltProject}/{Application.productName}_Data/Plugins/");
            
            //TODO: Check other targets
            if (target == BuildTarget.StandaloneWindows64)
                buildPluginsDir += "x86_64/";

            //Make sure it exists
            if (!Directory.Exists(buildPluginsDir))
                Directory.CreateDirectory(buildPluginsDir);
                
            //Go trough all installed engines
            foreach (BrowserEngine engine in BrowserEngines)
            {
                //Check if the engine has our build target
                if (!engine.BuildFiles.ContainsKey(target))
                    continue;
                
                //Get the location where we are copying all the files
                string buildFilesDir = Path.GetFullPath(engine.BuildFiles[target]);
                string buildFilesParent = Directory.GetParent(buildFilesDir)?.Name;
                
                //Get all files that aren't Unity .meta files
                string[] files = Directory.EnumerateFiles(buildFilesDir, "*.*", SearchOption.AllDirectories)
                    .Where(fileType => !fileType.EndsWith(".meta")).ToArray();
                int size = files.Length;

                //Now to copy all the files.
                //We need to keep the structure of the process
                for (int i = 0; i < size; i++)
                {
                    string file = files[i];
                    string destFileName = Path.GetFileName(file);
                    EditorUtility.DisplayProgressBar("Copying Browser Engine Files", 
                        $"Copying {destFileName}", (int)(i / size));
                    
                    string parentDirectory = "";
                    if (Directory.GetParent(file)?.Name != buildFilesParent)
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
            
            Debug.Log("Done!");
        }
    }
}