// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Shared;

namespace VoltstroStudios.UnityWebBrowser.Input
{
    /// <summary>
    ///     Abstraction layer for getting input
    /// </summary>
    public abstract class WebBrowserInputHandler : ScriptableObject
    {
        #region Position

        /// <summary>
        ///     Get the scroll
        /// </summary>
        /// <returns></returns>
        public abstract float GetScroll();

        /// <summary>
        ///     Get the current cursor position on the screen as a <see cref="Vector2" />
        /// </summary>
        /// <returns></returns>
        public abstract Vector2 GetCursorPos();

        #endregion

        #region Input

        /// <summary>
        ///     Get all keys that are down this frame
        /// </summary>
        /// <returns>Returns an array of <see cref="WindowsKey" /> that are up</returns>
        public abstract WindowsKey[] GetDownKeys();

        /// <summary>
        ///     Get all keys that are up this frame
        /// </summary>
        /// <returns>Returns an array of <see cref="WindowsKey" /> that are down</returns>
        public abstract WindowsKey[] GetUpKeys();

        /// <summary>
        ///     Gets the input buffer for this frame
        /// </summary>
        /// <returns></returns>
        public abstract string GetFrameInputBuffer();

        #endregion

        #region General

        /// <summary>
        ///     Called when inputs are started
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        ///     Called when inputs are stopped
        /// </summary>
        public abstract void OnStop();

        /// <summary>
        ///     Called when IME needs to be enabled
        /// </summary>
        public abstract void EnableIme(Vector2 location);

        /// <summary>
        ///     Called when IME is no longer needed
        /// </summary>
        public abstract void DisableIme();

        #endregion
    }
}