#if UNITY_EDITOR

using System.IO;
using System.Linq;
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

        public static Platform GetCurrentEditorPlatform()
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
    }
}

#endif