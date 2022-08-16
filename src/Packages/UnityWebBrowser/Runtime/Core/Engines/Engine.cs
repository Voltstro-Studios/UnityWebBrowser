// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Core.Engines
{
    public abstract class Engine : ScriptableObject
    {
        public abstract string GetEngineExecutableName();

#if UNITY_EDITOR
        public abstract string EngineFilesNotFoundError { get; }
        public abstract IEnumerable<EnginePlatformFiles> EngineFiles { get; }

        [Serializable]
        public struct EnginePlatformFiles
        {
            public Platform platform;

            public string engineFileLocation;
        }
#endif
    }
}