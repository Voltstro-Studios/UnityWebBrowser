using System;
using System.IO;
using UnityEngine;
using UnityWebBrowser.Shared.Core;

#if UNITY_EDITOR
using UnityWebBrowser.Editor.EngineManagement;
#endif

namespace UnityWebBrowser.Core.Engines
{
    [CreateAssetMenu(menuName = "UWB/UWB Engine Configuration", fileName = "New UWB Engine Configuration")]
    public class Engine : ScriptableObject
    {
        public string engineAppName;

#if UNITY_EDITOR

        public string engineFilesNotFoundError = "The engine files for this platform were not found! You may need to install a dedicated platform package.";
        public EngineFiles[] engineFiles;
        
        [Serializable]
        public struct EngineFiles
        {
            public Platform platform;

            public string engineFileLocation;
        }
#if UWB_ENGINE_PRJ
        public void OnValidate()
        {
            foreach (EngineFiles engineFile in engineFiles)
            {
                string path = EngineManager.GetEngineProcessFullPath(this, engineFile.platform);
                if(path == null || !File.Exists(path))
                    Debug.LogError($"Error with engines files for {name}");
            }
        }
#endif
        
#endif
    }
}
