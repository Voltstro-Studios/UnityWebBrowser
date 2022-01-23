using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using UnityWebBrowser.Logging;

#if UNITY_EDITOR
using UnityEditor;
using UnityWebBrowser.Editor.EngineManagement;
#endif

namespace UnityWebBrowser.Helper
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
        public static string GetBrowserEngineMainDirectory()
        {
#if UNITY_EDITOR
            return Path.GetFullPath($"{Directory.GetParent(Application.dataPath).FullName}/Library");
#else
			return Application.dataPath;
#endif
        }

        /// <summary>
        ///     Gets the folder that the UWB process application lives in
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEnginePath(string engine)
        {
            //Editor
#if UNITY_EDITOR
            BrowserEngine browserEngine = BrowserEngineManager.GetEngine(engine);

#if UNITY_EDITOR_WIN
            return Path.GetFullPath(browserEngine.BuildFiles.FirstOrDefault(x =>
                x.Key == UnityEditor.BuildTarget.StandaloneWindows ||
                x.Key == UnityEditor.BuildTarget.StandaloneWindows64).Value);
#elif UNITY_EDITOR_LINUX
            return Path.GetFullPath(browserEngine.BuildFiles.FirstOrDefault(x =>
                x.Key == BuildTarget.StandaloneLinux64).Value);
#endif

            //Player builds (Standalone)
#elif UNITY_STANDALONE
		    return Path.GetFullPath($"{Application.dataPath}/UWB/");
#endif
        }

        /// <summary>
        ///     Get a direct path to the UWB process application
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEngineProcessPath(string engine)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return $"{GetBrowserEnginePath(engine)}{engine}.exe";
#else
            return $"{GetBrowserEnginePath(engine)}{engine}";
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

            if (graphic == null)
                throw new ArgumentNullException(nameof(graphic), "Image cannot be null!");

            position = new Vector2();
            RectTransform uiImageObjectRect = graphic.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(uiImageObjectRect, screenPosition, null,
                    out Vector2 localCursor)) return false;

            Vector2 ptPivotCancelledLocation = new(localCursor.x - uiImageObjectRect.rect.x,
                localCursor.y - uiImageObjectRect.rect.y);
            Vector2 ptLocationRelativeToImageInScreenCoordinates =
                new(ptPivotCancelledLocation.x, ptPivotCancelledLocation.y);
            position.x = ptLocationRelativeToImageInScreenCoordinates.x / uiImageObjectRect.rect.width;
            position.y = -(ptLocationRelativeToImageInScreenCoordinates.y / uiImageObjectRect.rect.height) + 1;

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
        ///     Creates a <see cref="Process"/> for an engine
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="browserEngine"></param>
        /// <param name="browserEnginePath"></param>
        /// <param name="arguments"></param>
        /// <param name="onLogEvent"></param>
        /// <returns></returns>
        internal static Process CreateEngineProcess(IWebBrowserLogger logger, string browserEngine, string browserEnginePath, string arguments, 
            DataReceivedEventHandler onLogEvent)
        {
#if UNIX_SUPPORT && (UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX)
            if(UnixSupport.PermissionsManager.CheckAndSetIfNeededFileExecutablePermission(browserEnginePath))
                logger.Warn("UWB engine process did not have +rwx permissions! Engine process permission's were updated for the user.");
#endif
            
            Process process = new()
            {
                StartInfo = new ProcessStartInfo(browserEnginePath, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = GetBrowserEnginePath(browserEngine)
                },
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += onLogEvent;
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
        ///     Copies a <see cref="ReadOnlyMemory{T}"/> to a <see cref="NativeArray{T}"/>
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="copyTo"></param>
        /// <param name="token"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopySpanToNativeArray<T>(ReadOnlySpan<T> copyFrom, NativeArray<T> copyTo, CancellationToken token) where T : struct
        {
            for (int i = 0; i < copyFrom.Length; i++)
            {
                if(token.IsCancellationRequested)
                    return;
                
                copyTo[i] = copyFrom[i];
            }
        }
    }
}