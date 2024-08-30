// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

#nullable enable
namespace VoltstroStudios.UnityWebBrowser.Core.Engines
{
    public abstract class Engine : ScriptableObject
    {
        public abstract string GetEngineExecutableName();

        public virtual string GetEngineWorkingPath(Platform platform)
        {
            return Path.GetFullPath(GetEngineBasePath(platform));
        }
        
        public virtual string GetEngineAppPath(Platform platform)
        {
            return Path.GetFullPath(Path.Combine(GetEngineBasePath(platform), GetEngineExecutableName()));
        }

        public string GetEngineBasePath(Platform platform)
        {
            EnginePlatformFiles? engineFiles = EngineFiles.FirstOrDefault(x => x.platform == platform);
            if (engineFiles == null)
                throw new PlatformNotSupportedException();
            
#if UNITY_EDITOR
            return Path.Combine(engineFiles.Value.engineEditorLocation, engineFiles.Value.engineBaseAppLocation);
#else 
            return Path.Combine(Application.dataPath, engineFiles.Value.engineRuntimeLocation, engineFiles.Value.engineBaseAppLocation);
#endif

        }
        
        public abstract IEnumerable<EnginePlatformFiles> EngineFiles { get; }

        [Serializable]
        public struct EnginePlatformFiles
        {
            /// <summary>
            ///     <see cref="Platform"/> this set of files are for
            /// </summary>
            public Platform platform;

            /// <summary>
            ///     Location of the app itself actually lives.
            ///     Builds on top off of <see cref="engineRuntimeLocation"/> or <see cref="engineEditorLocation"/> (in editor mode)
            /// </summary>
            public string engineBaseAppLocation;
            
            /// <summary>
            ///     Runtime location for the engine files
            /// </summary>
            public string engineRuntimeLocation;

#if UNITY_EDITOR
            
            /// <summary>
            ///     Editor location for the engine files
            /// </summary>
            [FormerlySerializedAs("engineFileLocation")]
            public string engineEditorLocation;
            
            [HideInInspector]
            [Obsolete("This field is no longer used, it has been renamed to engineEditorLocation to better describe what this field is now used for.")]
            public string engineFileLocation;
            
#endif
        }
        
        [Obsolete]
        public abstract string EngineFilesNotFoundError { get; }
    }
}