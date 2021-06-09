using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityWebBrowser.Editor
{
    public static class BrowserEngineManager
    {
        private static readonly List<BrowserEngine> BrowserEngines = new List<BrowserEngine>();

        internal static List<BrowserEngine> Engines => BrowserEngines;

        public static void AddBrowserEngine(BrowserEngine engine)
        {
            if (!CheckIfAppExists(engine))
            {
                Debug.LogError($"{engine.EngineName} is setup incorrectly!");
                return;
            }
            
            BrowserEngines.Add(engine);
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
    }
}