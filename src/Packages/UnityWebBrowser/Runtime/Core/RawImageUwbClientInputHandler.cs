// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using VoltstroStudios.UnityWebBrowser.Helper;
using VoltstroStudios.UnityWebBrowser.Input;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Core
{
    /// <summary>
    ///     Input handler for <see cref="RawImageUwbClientManager" />.
    /// </summary>
    public abstract class RawImageUwbClientInputHandler : RawImageUwbClientManager,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        /// <summary>
        ///     The <see cref="WebBrowserInputHandler" /> to use
        /// </summary>
        [Tooltip("The input handler to use")] public WebBrowserInputHandler inputHandler;

        /// <summary>
        ///     Disable usage of mouse
        /// </summary>
        [Tooltip("Disable usage of mouse")] public bool disableMouseInputs;

        /// <summary>
        ///     Disable usage of keyboard
        /// </summary>
        [Tooltip("Disable usage of keyboard")] public bool disableKeyboardInputs;

        private Coroutine keyboardAndMouseHandlerCoroutine;
        private Vector2 lastSuccessfulMousePositionSent;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (disableMouseInputs)
                return;

            MouseClickType clickType = eventData.button switch
            {
                PointerEventData.InputButton.Left => MouseClickType.Left,
                PointerEventData.InputButton.Right => MouseClickType.Right,
                PointerEventData.InputButton.Middle => MouseClickType.Middle,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (GetMousePosition(out Vector2 pos))
                browserClient.SendMouseClick(pos, eventData.clickCount, clickType, MouseEventType.Down);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (browserClient is { IsConnected: false })
                return;

            keyboardAndMouseHandlerCoroutine = StartCoroutine(KeyboardAndMouseHandler());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopKeyboardAndMouseHandler();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (disableMouseInputs)
                return;

            MouseClickType clickType = eventData.button switch
            {
                PointerEventData.InputButton.Left => MouseClickType.Left,
                PointerEventData.InputButton.Right => MouseClickType.Right,
                PointerEventData.InputButton.Middle => MouseClickType.Middle,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (GetMousePosition(out Vector2 pos))
                browserClient.SendMouseClick(pos, eventData.clickCount, clickType, MouseEventType.Up);
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (inputHandler == null)
                throw new NullReferenceException("The input handler is null! You need to assign it in the editor!");
            
            browserClient.OnInputFocus += OnClientInput;
        }

        private void OnClientInput(bool focused)
        {
            if (focused)
            {
                if (!GetMousePosition(out Vector2 pos)) return;
                pos.x /= 1.5f;
                inputHandler.EnableIme(pos);
                return;
            }
            
            inputHandler.DisableIme();
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            StopKeyboardAndMouseHandler();
        }

        /// <summary>
        ///     Gets the current mouse position on the image
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Returns true if the mouse is in the image.</returns>
        public bool GetMousePosition(out Vector2 pos)
        {
            Vector2 mousePos = inputHandler.GetCursorPos();

            if (WebBrowserUtils.GetScreenPointToLocalPositionDeltaOnImage(image, mousePos, out pos))
            {
                Texture imageTexture = image.texture;
                pos.x *= imageTexture.width;
                pos.y *= imageTexture.height;

                return true;
            }

            return false;
        }

        private void StopKeyboardAndMouseHandler()
        {
            if (keyboardAndMouseHandlerCoroutine != null)
            {
                StopCoroutine(keyboardAndMouseHandlerCoroutine);
                inputHandler.OnStop();
            }
        }

        private IEnumerator KeyboardAndMouseHandler()
        {
            inputHandler.OnStart();

            while (Application.isPlaying)
            {
                yield return 0;

                if (!browserClient.ReadySignalReceived || !browserClient.IsConnected 
                                                       || browserClient.HasDisposed)
                    continue;

                if (disableMouseInputs && disableKeyboardInputs)
                    continue;

                if (GetMousePosition(out Vector2 pos))
                {
                    if (!disableMouseInputs)
                    {
                        //Mouse position
                        if (lastSuccessfulMousePositionSent != pos)
                        {
                            browserClient.SendMouseMove(pos);
                            lastSuccessfulMousePositionSent = pos;
                        }

                        //Mouse scroll
                        float scroll = inputHandler.GetScroll();
                        scroll *= browserClient.BrowserTexture.height;

                        if (scroll != 0)
                            browserClient.SendMouseScroll(pos, (int)scroll);
                    }

                    if (!disableKeyboardInputs)
                    {
                        //Input
                        WindowsKey[] keysDown = inputHandler.GetDownKeys();
                        WindowsKey[] keysUp = inputHandler.GetUpKeys();
                        string inputBuffer = inputHandler.GetFrameInputBuffer();

                        if (keysDown.Length > 0 || keysUp.Length > 0 || inputBuffer.Length > 0)
                            browserClient.SendKeyboardControls(keysDown, keysUp, inputBuffer.ToCharArray());
                    }
                }
            }

            inputHandler.OnStop();
        }
    }
}