// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

//Defines
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX && !UWB_DOCS

//We need the Unix support package installed on UNIX systems
#if !UNIX_SUPPORT
#error Need UNIX support package!
#else
#define UWB_NEED_UNIX
#endif

#endif

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Logging;
#if UWB_NEED_UNIX
using VoltstroStudios.UnityWebBrowser.UnixSupport;
#endif
#if UNITY_EDITOR
using VoltstroStudios.UnityWebBrowser.Editor.EngineManagement;
#endif

namespace VoltstroStudios.UnityWebBrowser.Helper
{
    /// <summary>
    ///     Provides Utils to be used by the web browser
    /// </summary>
    [Preserve]
    public static class WebBrowserUtils
    {
        /// <summary>
        ///     Gets the main directory where logs and cache may be stored
        /// </summary>
        /// <returns></returns>
        public static string GetAdditionFilesDirectory()
        {
#if UNITY_EDITOR
            return Path.GetFullPath($"{Directory.GetParent(Application.dataPath).FullName}/Library");
#elif UNITY_STANDALONE_OSX
            return Application.persistentDataPath;
#else
			return Application.dataPath;
#endif
        }

        /// <summary>
        ///     Gets the folder that the UWB process application lives in
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEnginePath(Engine engine)
        {
            //Editor
#if UNITY_EDITOR
            return EngineManager.GetEngineDirectory(engine);

            //Player builds (Standalone)
#elif UNITY_STANDALONE_OSX
            return Path.GetFullPath($"{Application.dataPath}/Resources/Data/UWB/");
#elif UNITY_STANDALONE
		    return Path.GetFullPath($"{Application.dataPath}/UWB/");
#endif
        }

        /// <summary>
        ///     Get a direct path to the UWB process application
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEngineProcessPath(Engine engine)
        {
#if UNITY_EDITOR
            return EngineManager.GetEngineProcessFullPath(engine);
#else
            string path = $"{GetBrowserEnginePath(null)}/{engine.GetEngineExecutableName()}";
#if UNITY_STANDALONE_WIN
            path += ".exe";
#endif
            return  Path.GetFullPath(path);
#endif
        }

        /// <summary>
        ///     Gets the local position delta (0 -> 1) from a screen position on a <see cref="Graphic" />
        ///     from a top-left origin point
        ///     <para>
        ///         To calculate the pixel position,
        ///         do <see cref="Vector2.x" /> * [Desired height] and <see cref="Vector2.y" /> * [Desired Width]
        ///     </para>
        /// </summary>
        /// <param name="graphic"><see cref="Graphic" /> that you want to calculate the local position on</param>
        /// <param name="screenPosition">The screen position</param>
        /// <param name="position">The local delta position</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the graphic is null</exception>
        public static bool GetScreenPointToLocalPositionDeltaOnImage(Graphic graphic, Vector2 screenPosition,
            out Vector2 position)
        {
            //This was a pain in the ass to figure out how to do, I never want anything to do with mouses and UI elements ever again.

            //The main pain was that CEF uses a top left origin point, not center like Unity, but turns out its dog shit
            //simple to do and that I am terrible at maths

            //There probs something here that could be done better, if you know, send in a PR
            //Based off: http://answers.unity.com/answers/1455168/view.html

            if (graphic.Equals(null))
                throw new ArgumentNullException(nameof(graphic), "Image cannot be null!");

            Camera camera = graphic.canvas.worldCamera;

            position = new Vector2();
            RectTransform uiImageObjectRectTransform = graphic.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(uiImageObjectRectTransform, screenPosition,
                    camera,
                    out Vector2 localCursor))
                return false;

            Rect uiImageObjectRect = uiImageObjectRectTransform.rect;
            Vector2 ptPivotCancelledLocation = new(localCursor.x - uiImageObjectRect.x,
                localCursor.y - uiImageObjectRect.y);
            Vector2 ptLocationRelativeToImageInScreenCoordinates =
                new(ptPivotCancelledLocation.x, ptPivotCancelledLocation.y);
            position.x = ptLocationRelativeToImageInScreenCoordinates.x / uiImageObjectRect.width;
            position.y = -(ptLocationRelativeToImageInScreenCoordinates.y / uiImageObjectRect.height) + 1;

            return true;
        }

        /// <summary>
        ///     Converts a <see cref="Color32" /> to hex
        /// </summary>
        /// <param name="color"></param>
        internal static string ColorToHex(Color32 color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }

        /// <summary>
        ///     Creates a <see cref="Process" /> for an engine
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="engine"></param>
        /// <param name="arguments"></param>
        /// <param name="onLogEvent"></param>
        /// <param name="onErrorLogEvent"></param>
        /// <returns></returns>
        internal static Process CreateEngineProcess(IWebBrowserLogger logger, Engine engine, string arguments,
            DataReceivedEventHandler onLogEvent, DataReceivedEventHandler onErrorLogEvent)
        {
            string engineFullProcessPath = GetBrowserEngineProcessPath(engine);
            string engineDirectory = GetBrowserEnginePath(engine);

            logger.Debug($"Process Path: '{engineFullProcessPath}'\nWorking: '{engineDirectory}'");
            logger.Debug($"Arguments: '{arguments}'");

#if UWB_NEED_UNIX
            if (PermissionsManager.CheckAndSetIfNeededFileExecutablePermission(engineFullProcessPath))
                logger.Warn(
                    "UWB engine process did not have +rwx permissions! Engine process permission's were updated for the user.");
#endif

            Process process = new()
            {
                StartInfo = new ProcessStartInfo(engineFullProcessPath, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = engineDirectory
                },
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += onLogEvent;
            process.ErrorDataReceived += onErrorLogEvent;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

        /// <summary>
        ///     Sets every single pixel in a <see cref="Texture2D" /> to one <see cref="Color32" />
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        internal static void SetAllTextureColorToOne(Texture2D texture, Color32 color)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture), "Texture cannot be null!");

            Color32[] colors = new Color32[texture.width * texture.height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            texture.SetPixels32(colors);
            texture.Apply();
        }

        /// <summary>
        ///     Copies a <see cref="ReadOnlyMemory{T}" /> to a <see cref="NativeArray{T}" />
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="token"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopySpanToNativeArray<T>(ReadOnlySpan<T> copyFrom, NativeArray<T> copyTo,
            CancellationToken token) where T : struct
        {
            for (int i = 0; i < copyFrom.Length; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                copyTo[i] = copyFrom[i];
            }
        }
    }
}