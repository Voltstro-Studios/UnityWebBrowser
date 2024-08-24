// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
#if UNITY_EDITOR
using VoltstroStudios.UnityWebBrowser.Editor.EngineManagement;
#endif

namespace VoltstroStudios.UnityWebBrowser.Core.Engines
{
    [CreateAssetMenu(menuName = "UWB/UWB Engine Configuration", fileName = "New UWB Engine Configuration")]
    public class EngineConfiguration : Engine
    {
        /// <summary>
        ///     Array of <see cref="Engine.EnginePlatformFiles"/>
        /// </summary>
        public EnginePlatformFiles[] engineFiles;
        
        /// <summary>
        ///     Main application app name
        /// </summary>
        public string engineAppName;

        public override string GetEngineExecutableName()
        {
#if UNITY_STANDALONE_WIN
            return engineAppName + ".exe";
#else 
            return engineAppName;
#endif
        }
        
        [Obsolete]
        [HideInInspector]
        public string engineFilesNotFoundError =
            "The engine files for this platform were not found! You may need to install a dedicated platform package.";

        [Obsolete]
        public override string EngineFilesNotFoundError => null;
        
        public override IEnumerable<EnginePlatformFiles> EngineFiles => engineFiles;
    }
}