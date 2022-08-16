// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using VoltstroStudios.UnityWebBrowser.Editor.EngineManagement;
#endif

namespace VoltstroStudios.UnityWebBrowser.Core.Engines
{
    [CreateAssetMenu(menuName = "UWB/UWB Engine Configuration", fileName = "New UWB Engine Configuration")]
    public class EngineConfiguration : Engine
    {
        public string engineAppName;

        public override string GetEngineExecutableName()
        {
            return engineAppName;
        }

#if UNITY_EDITOR

        public string engineFilesNotFoundError =
            "The engine files for this platform were not found! You may need to install a dedicated platform package.";

        public EnginePlatformFiles[] engineFiles;

        public override string EngineFilesNotFoundError => engineFilesNotFoundError;
        public override IEnumerable<EnginePlatformFiles> EngineFiles => engineFiles;

#if UWB_ENGINE_PRJ
        public void OnValidate()
        {
            foreach (EnginePlatformFiles engineFile in engineFiles)
            {
                string path = EngineManager.GetEngineProcessFullPath(this, engineFile.platform);
                if (path == null || !File.Exists(path))
                    Debug.LogError($"Error with engines files for {name} on platform {engineFile.platform}!");
            }
        }
#endif

#endif
    }
}