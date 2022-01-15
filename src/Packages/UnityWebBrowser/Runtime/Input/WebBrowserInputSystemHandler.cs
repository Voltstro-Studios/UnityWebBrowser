#if ENABLE_INPUT_SYSTEM

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Input
{
    [CreateAssetMenu(fileName = "Input System Handler", menuName = "UWB/Inputs/Input System Handler")]
    public sealed class WebBrowserInputSystemHandler : WebBrowserInputHandler
    {
        [Header("Scroll Input")]
        public InputAction scrollInput;

        public float scrollValue = 0.2f;

        [Header("Pointer Position")]
        public InputAction pointPosition;

        private Keyboard keyboard;
        private string inputBuffer = string.Empty;
        private readonly List<WindowsKey> keysDown = new();
        private readonly List<WindowsKey> keysUp = new();

        public override float GetScroll()
        {
            //Mouse scroll wheel in the new input system is fucked, its value is either 120 or -120,
            //no in-between or -1.0 to 1.0 like the old input system. Why Unity.
            //While there are forum post talking about this, nothing is from Unity themselves about the issue.
            float scroll = scrollInput.ReadValue<Vector2>().y;
            scroll = Mathf.Clamp(scroll, -scrollValue, scrollValue);

            return scroll;
        }

        public override Vector2 GetCursorPos()
        {
            return pointPosition.ReadValue<Vector2>();
        }

        public override WindowsKey[] GetDownKeys()
        {
            keysDown.Clear();
            foreach (KeyControl key in keyboard.allKeys)
            {
                try
                {
                    if (key.wasPressedThisFrame)
                        keysDown.Add(key.keyCode.UnityKeyToWindowKey());
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
            foreach (KeyControl key in keyboard.allKeys)
            {
                try
                {
                    if (key.wasReleasedThisFrame)
                        keysUp.Add(key.keyCode.UnityKeyToWindowKey());
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
            string buffer = inputBuffer;
            inputBuffer = string.Empty;
            return buffer;
        }

        public override void OnStart()
        {
            scrollInput.Enable();
            pointPosition.Enable();

            keyboard = Keyboard.current;
            keyboard.onTextInput += OnTextEnter;
            inputBuffer = string.Empty;
            
            keysDown.Clear();
            keysUp.Clear();
        }

        private void OnTextEnter(char character)
        {
            inputBuffer += character;
        }

        public override void OnStop()
        {
            keyboard.onTextInput -= OnTextEnter;
            scrollInput.Disable();
            pointPosition.Disable();

            keyboard = null;
        }
    }
}

#endif