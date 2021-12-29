using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityWebBrowser.Input;
using UnityWebBrowser.Shared.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace UnityWebBrowser
{
    /// <summary>
    ///     This class handles the UI side of UWB. It is also the <see cref="MonoBehaviour"/> holder for the <see cref="WebBrowserClient"/>,
    ///     which is where all the actual magic happens.
    ///     <para><see cref="WebBrowserUI"/> will also automatically handle setting up the <see cref="RawImage"/> for you.</para>
    ///     <para>If you need to invoke events on UWB process, you can do it from here.</para>
    /// </summary>
    [AddComponentMenu("UWB/Web Browser UI")]
    [HelpURL("https://github.com/Voltstro-Studios/UnityWebBrowser")]
    [RequireComponent(typeof(RawImage))]
    public sealed class WebBrowserUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        /// <summary>
        ///     The <see cref="WebBrowserClient" />, what handles the communication between the UWB engine and Unity
        /// </summary>
        [Tooltip("The browser client, what handles the communication between the UWB engine and Unity")]
        public WebBrowserClient browserClient = new();

        /// <summary>
        ///     Support automatically handling fullscreen events.
        ///     <para>If enabled, it will make the <see cref="image"/> the full size of the window when the browser gets a fullscreen event.</para>
        /// </summary>
        [Tooltip("Support automatically handling fullscreen events.\nIf enabled, it will make the image the full size of the window when the browser gets a fullscreen event.")]
        public bool supportFullscreen = true;

        /// <summary>
        ///     What objects to hide when the browser wants to be in fullscreen mode
        /// </summary>
        [Tooltip("What objects to hide when the browser wants to be in fullscreen mode")]
        public GameObject[] hideOnFullscreen;

        private RawImage image;
        private Coroutine pointerKeyboardHandler;
        private Vector2 lastSuccessfulMousePositionSent;

        private Vector2 lastImageSize;
        private Vector2 lastAnchorMin;
        private Vector2 lastAnchorMax;
        private Vector2 lastImagePosition;

        private CancellationTokenSource cancellationToken;

#if ENABLE_INPUT_SYSTEM
        private string currentInputBuffer;
#else
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
		#error Unity Web Browser on linux does not support old input system!
#endif
        private static readonly KeyCode[] Keymap = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
#endif

        /// <summary>
        ///     Makes the browser go back a page
        /// </summary>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void GoBack()
        {
            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.GoBack();
        }

        /// <summary>
        ///     Make the browser go forward a page
        /// </summary>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void GoForward()
        {
            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.GoForward();
        }

        /// <summary>
        ///     Makes the browser go to a url
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void NavigateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.LoadUrl(url);
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void Refresh()
        {
            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.Refresh();
        }

        /// <summary>
        ///     Loads HTML code
        /// </summary>
        /// <param name="html"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void LoadHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                throw new ArgumentNullException(nameof(html));

            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS
        /// </summary>
        /// <param name="js"></param>
        public void ExecuteJs(string js)
        {
            if (string.IsNullOrWhiteSpace(js))
                throw new ArgumentNullException(nameof(js));

            if (!browserClient.IsConnected)
                throw new WebBrowserIsNotConnectedException("The web browser is not ready right now!");

            browserClient.ExecuteJs(js);
        }

        private void Start()
        {
            //Start the browser client
            browserClient.Init();
            cancellationToken = new CancellationTokenSource();
            
            //Run the pixel loop
            _ = Task.Run(() => browserClient.PixelDataLoop(cancellationToken.Token));

            image = GetComponent<RawImage>();
            image.texture = browserClient.BrowserTexture;
            image.uvRect = new Rect(0f, 0f, 1f, -1f);
            
            browserClient.OnFullscreen += ToggleFullscreen;

#if ENABLE_INPUT_SYSTEM
            Keyboard.current.onTextInput += c => currentInputBuffer += c;
#endif
        }
        
        private void FixedUpdate()
        {
            //We load the pixel data into the texture at a fixed rate
            browserClient.LoadTextureData();
        }

        private void OnDestroy()
        {
            cancellationToken.Cancel();
            browserClient.Dispose();
        }

        /// <summary>
        ///     Toggles fullscreen.
        ///     <para>Requires <see cref="supportFullscreen"/> to be enabled.</para>
        /// </summary>
        /// <param name="fullscreen">Go into fullscreen or not.</param>
        public void ToggleFullscreen(bool fullscreen)
        {
            if (supportFullscreen)
            {
                if (fullscreen)
                {
                    foreach (GameObject obj in hideOnFullscreen)
                        obj.SetActive(false);

                    RectTransform imageRectTransform = image.rectTransform;
                    lastImageSize = imageRectTransform.sizeDelta;
                    lastAnchorMax = imageRectTransform.anchorMax;
                    lastAnchorMin = imageRectTransform.anchorMin;
                    lastImagePosition = imageRectTransform.anchoredPosition;
                    
                    imageRectTransform.anchoredPosition = Vector2.zero;
                    imageRectTransform.anchorMin = Vector2.zero;
                    imageRectTransform.anchorMax = Vector2.one;
                    imageRectTransform.sizeDelta = Vector2.zero;
                }
                else
                {
                    foreach (GameObject obj in hideOnFullscreen)
                        obj.SetActive(true);
                    
                    RectTransform imageRectTransform = image.rectTransform;
                    imageRectTransform.anchoredPosition = lastImagePosition;
                    imageRectTransform.anchorMin = lastAnchorMin;
                    imageRectTransform.anchorMax = lastAnchorMax;
                    imageRectTransform.sizeDelta = lastImageSize;
                }
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            //Use this to check user settings
            if (browserClient.ipcSettings.preferPipes)
            {
                if (string.IsNullOrWhiteSpace(browserClient.ipcSettings.inPipeName))
                {
                    browserClient.ipcSettings.inPipeName = "UnityWebBrowserIn";
                    Debug.LogError("The in pipe name cannot be null or white space!");
                }

                if (string.IsNullOrWhiteSpace(browserClient.ipcSettings.outPipeName))
                {
                    browserClient.ipcSettings.outPipeName = "UnityWebBrowserOut";
                    Debug.LogError("The out pipe name cannot be null or white space!");
                }

                if (browserClient.ipcSettings.inPipeName == browserClient.ipcSettings.outPipeName)
                {
                    browserClient.ipcSettings.inPipeName = "UnityWebBrowserIn";
                    browserClient.ipcSettings.outPipeName = "UnityWebBrowserOut";
                    Debug.LogError("The pipe names cannot be the same!");
                }
            }
            else
            {
                if (browserClient.ipcSettings.inPort == browserClient.ipcSettings.outPort)
                {
                    browserClient.ipcSettings.outPort = 5555;
                    browserClient.ipcSettings.inPort = 5556;
                    Debug.LogError("The in and out IPC ports cannot be the same!");
                }

                if (browserClient.remoteDebugging &&
                    browserClient.remoteDebuggingPort == browserClient.ipcSettings.inPort ||
                    browserClient.remoteDebuggingPort == browserClient.ipcSettings.outPort)
                {
                    browserClient.remoteDebuggingPort = 9022;
                    browserClient.ipcSettings.outPort = 5555;
                    browserClient.ipcSettings.inPort = 5556;
                    Debug.LogError("The remote debugging port cannot be the same as the in or out IPC ports!");
                }
            }
        }

#endif

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerKeyboardHandler = StartCoroutine(HandlerPointerAndKeyboardData());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopCoroutine(pointerKeyboardHandler);
        }

        private IEnumerator HandlerPointerAndKeyboardData()
        {
            while (Application.isPlaying)
            {
                List<int> keysDown = new List<int>();
                List<int> keysUp = new List<int>();

#if ENABLE_INPUT_SYSTEM
                //We need to find all keys that were pressed and released
                foreach (KeyControl key in Keyboard.current.allKeys)
                    try
                    {
                        if (key.wasPressedThisFrame)
                            keysDown.Add((int)key.keyCode.UnityKeyToWindowKey());

                        if (key.wasReleasedThisFrame)
                            keysUp.Add((int)key.keyCode.UnityKeyToWindowKey());
                    }
                    catch (Exception)
                    {
                        browserClient.logger.Warn($"Unsupported key conversion attempted! Key: {key}");
                    }

                //Send our input if any key is down or up
                if (keysDown.Count != 0 || keysUp.Count != 0 || !string.IsNullOrEmpty(currentInputBuffer))
                {
                    browserClient.SendKeyboardControls(keysDown.ToArray(), keysUp.ToArray(), currentInputBuffer);
                    currentInputBuffer = "";
                }
#else
                //We need to find all keys that were pressed and released
                foreach (KeyCode key in Keymap)
                {
                    //Why are mouse buttons considered key codes???
                    if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2
                        || key == KeyCode.Mouse3 || key == KeyCode.Mouse4
                        || key == KeyCode.Mouse5 || key == KeyCode.Mouse6)
                        continue;

                    try
                    {
                        if (UnityEngine.Input.GetKeyDown(key))
                            keysDown.Add((int) key.UnityKeyCodeToWindowKey());
                        if (UnityEngine.Input.GetKeyUp(key))
                            keysUp.Add((int) key.UnityKeyCodeToWindowKey());
                    }
                    catch (Exception)
                    {
                        browserClient.LogWarning($"Unsupported key conversion attempted! KeyCode: {key}");
                    }
                }

                if (keysDown.Count != 0 || keysUp.Count != 0 || !string.IsNullOrEmpty(UnityEngine.Input.inputString))
                    browserClient.SendKeyboardControls(keysDown.ToArray(), keysUp.ToArray(),
                        UnityEngine.Input.inputString);
#endif
                if (GetMousePosition(out Vector2 pos))
                {
                    if (lastSuccessfulMousePositionSent != pos)
                    {
                        browserClient.SendMouseMove(pos);
                        lastSuccessfulMousePositionSent = pos;
                    }

                    //Mouse scroll
#if ENABLE_INPUT_SYSTEM
                    //Mouse scroll wheel in the new input system is fucked, its value is either 120 or -120,
                    //no in-between or -1.0 to 1.0 like the old input system. Why Unity.
                    //And nobody knows anything about it because I guess no one uses the scroll wheel like this
                    float scroll = Mouse.current.scroll.y.ReadValue();
                    if (scroll > 0)
                        scroll = 0.2f;
                    if (scroll < 0)
                        scroll = -0.2f;
#else
                    float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
#endif
                    scroll *= browserClient.BrowserTexture.height;

                    if (scroll != 0)
                        browserClient.SendMouseScroll((int)pos.x, (int)pos.y, (int)scroll);
                }

                yield return 0;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
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

        public void OnPointerUp(PointerEventData eventData)
        {
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

        private bool GetMousePosition(out Vector2 pos)
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 mousePos = Mouse.current.position.ReadValue();
#else
            Vector2 mousePos = UnityEngine.Input.mousePosition;
#endif

            if (WebBrowserUtils.GetScreenPointToLocalPositionDeltaOnImage(image, mousePos, out pos))
            {
                pos.x *= browserClient.Resolution.Width;
                pos.y *= browserClient.Resolution.Height;

                return true;
            }

            return false;
        }
    }
}