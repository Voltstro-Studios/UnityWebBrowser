// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Shared;

namespace VoltstroStudios.UnityWebBrowser.Input
{
    /// <summary>
    ///     Input handler using Unity's old <see cref="UnityEngine.Input" />
    /// </summary>
    [CreateAssetMenu(fileName = "Old Input Handler", menuName = "UWB/Inputs/Old Input Handler")]
    public sealed class WebBrowserOldInputHandler : WebBrowserInputHandler
    {
        private static readonly KeyCode[] Keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

        /// <summary>
        ///     The name of the axis for the scroll
        /// </summary>
        [Tooltip("The name of the axis for the scroll")]
        public string scrollAxisName = "Mouse ScrollWheel";

        /// <summary>
        ///     How much sensitivity for the scroll
        /// </summary>
        [Tooltip("How much sensitivity for the scroll")]
        public float scrollSensitivity = 2.0f;

        private readonly List<WindowsKey> keysDown = new();
        private readonly List<WindowsKey> keysUp = new();

        private IMECompositionMode compositionMode;

        public override float GetScroll()
        {
            return UnityEngine.Input.GetAxis(scrollAxisName) * scrollSensitivity;
        }

        public override Vector2 GetCursorPos()
        {
            return UnityEngine.Input.mousePosition;
        }

        public override WindowsKey[] GetDownKeys()
        {
            keysDown.Clear();

            foreach (KeyCode key in Keymap)
            {
                //Why are mouse buttons considered key codes???
                if (key is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2 or KeyCode.Mouse3 or KeyCode.Mouse4
                    or KeyCode.Mouse5 or KeyCode.Mouse6)
                    continue;

                try
                {
                    if (UnityEngine.Input.GetKeyDown(key))
                        keysDown.Add(key.UnityKeyCodeToWindowKey());
                }
                catch (ArgumentOutOfRangeException)
                {
                    //Safe to ignore
                }
            }

            return keysDown.ToArray();
        }

        public override WindowsKey[] GetUpKeys()
        {
            keysUp.Clear();

            foreach (KeyCode key in Keymap)
            {
                //Why are mouse buttons considered key codes???
                if (key is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2 or KeyCode.Mouse3 or KeyCode.Mouse4
                    or KeyCode.Mouse5 or KeyCode.Mouse6)
                    continue;

                try
                {
                    if (UnityEngine.Input.GetKeyUp(key))
                        keysUp.Add(key.UnityKeyCodeToWindowKey());
                }
                catch (ArgumentOutOfRangeException)
                {
                    //Safe to ignore
                }
            }

            return keysUp.ToArray();
        }

        public override string GetFrameInputBuffer()
        {
            return UnityEngine.Input.inputString;
        }

        public override void OnStart()
        {
            keysDown.Clear();
            keysUp.Clear();
        }

        public override void OnStop()
        {
        }

        public override void EnableIme(Vector2 location)
        {
            compositionMode = UnityEngine.Input.imeCompositionMode;
            UnityEngine.Input.imeCompositionMode = IMECompositionMode.On;
            UnityEngine.Input.compositionCursorPos = location;
        }

        public override void DisableIme()
        {
            UnityEngine.Input.imeCompositionMode = compositionMode;
        }
    }
}