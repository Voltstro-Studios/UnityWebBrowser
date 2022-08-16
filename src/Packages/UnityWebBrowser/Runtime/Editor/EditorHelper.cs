// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using UnityEditor;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

namespace VoltstroStudios.UnityWebBrowser.Editor
{
    public static class EditorHelper
    {
        public static List<T> FindAssetsByType<T>() where T : Object
        {
            List<T> assets = new();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) assets.Add(asset);
            }

            return assets;
        }

        public static Platform UnityBuildTargetToPlatform(this BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneLinux64:
                    return Platform.Linux64;
                case BuildTarget.StandaloneWindows64:
                    return Platform.Windows64;
                case BuildTarget.StandaloneOSX:
                    return Platform.MacOS;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

#endif