using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityWebBrowser.EventData;
using UnityWebBrowser.Input;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace UnityWebBrowser
{
	[AddComponentMenu("Browser/Web Browser UI")]
	[HelpURL("https://github.com/Voltstro-Studios/UnityWebBrowser")]
	[RequireComponent(typeof(RawImage))]
	public sealed class WebBrowserUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		/// <summary>
		///		The <see cref="WebBrowserClient"/>, what handles the communication between the CEF process and Unity
		/// </summary>
		[Tooltip("The browser client, what handles the communication between the CEF process and Unity")]
		public WebBrowserClient browserClient = new WebBrowserClient();

		private RawImage image;
		private Coroutine pointerKeyboardHandler;
		private Vector2 lastSuccessfulMousePositionSent;

#if ENABLE_INPUT_SYSTEM
		private string currentInputBuffer;
#else
#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
		#error Unity Web Browser on linux does not support old input system!
#endif
		private static readonly KeyCode[] Keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));
#endif

		/// <summary>
		///		Makes the browser go back a page
		/// </summary>
		/// <exception cref="WebBrowserNotReadyException"></exception>
		public void GoBack()
		{
			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.SendButtonEvent(ButtonType.Back);
		}

		/// <summary>
		///		Make the browser go forward a page
		/// </summary>
		/// <exception cref="WebBrowserNotReadyException"></exception>
		public void GoForward()
		{
			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.SendButtonEvent(ButtonType.Forward);
		}

		/// <summary>
		///		Makes the browser go to a url
		/// </summary>
		/// <param name="url"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="WebBrowserNotReadyException"></exception>
		public void NavigateUrl(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.SendButtonEvent(ButtonType.NavigateUrl, url);
		}

		/// <summary>
		///		Refreshes the browser
		/// </summary>
		/// <exception cref="WebBrowserNotReadyException"></exception>
		public void Refresh()
		{
			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.SendButtonEvent(ButtonType.Refresh);
		}

		/// <summary>
		///		Loads HTML code
		/// </summary>
		/// <param name="html"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="WebBrowserNotReadyException"></exception>
		public void LoadHtml(string html)
		{
			if (string.IsNullOrWhiteSpace(html))
				throw new ArgumentNullException(nameof(html));

			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.LoadHtmlEvent(html);
		}

		/// <summary>
		///		Executes JS
		/// </summary>
		/// <param name="js"></param>
		public void ExecuteJs(string js)
		{
			if (string.IsNullOrWhiteSpace(js))
				throw new ArgumentNullException(nameof(js));

			if (!browserClient.IsRunning)
				throw new WebBrowserNotReadyException("The web browser is not ready right now!");

			browserClient.ExecuteJsEvent(js);
		}

		private void Start()
		{
			//Start the browser client
			browserClient.Init();
			StartCoroutine(browserClient.Start());

			image = GetComponent<RawImage>();
			image.texture = browserClient.BrowserTexture;
			image.uvRect = new Rect(0f, 0f, 1f, -1f);

#if ENABLE_INPUT_SYSTEM
			Keyboard.current.onTextInput += c => currentInputBuffer += c;
#endif
		}

		private void OnDestroy()
		{
			browserClient.Dispose();
		}

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
				{
					try
					{
						if (key.wasPressedThisFrame)
							keysDown.Add((int) key.keyCode.UnityKeyToWindowKey());

						if (key.wasReleasedThisFrame)
							keysUp.Add((int) key.keyCode.UnityKeyToWindowKey());
					}
					catch (Exception)
					{
						browserClient.LogWarning($"Unsupported key conversion attempted! Key: {key}");
					}
				}

				//Send our input if any key is down or up
				if (keysDown.Count != 0 || keysUp.Count != 0 || !string.IsNullOrEmpty(currentInputBuffer))
				{
					browserClient.SendKeyboardEvent(keysDown.ToArray(), keysUp.ToArray(), currentInputBuffer);
					currentInputBuffer = "";
				}
#else
				//We need to find all keys that were pressed and released
				foreach (KeyCode key in Keymap)
				{
					//Why are mouse buttons considered key codes???
					if(key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2 
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

				if(keysDown.Count != 0 || keysUp.Count != 0 || !string.IsNullOrEmpty(UnityEngine.Input.inputString))
					browserClient.SendKeyboardEvent(keysDown.ToArray(), keysUp.ToArray(), UnityEngine.Input.inputString);
#endif
				if (GetMousePosition(out Vector2 pos))
				{
					if (lastSuccessfulMousePositionSent != pos)
					{
						browserClient.SendMouseMoveEvent(pos);
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

					if(scroll != 0)
						browserClient.SendMouseScrollEvent((int)pos.x, (int)pos.y, (int)scroll);
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

			if(GetMousePosition(out Vector2 pos))
				browserClient.SendMouseClickEvent(pos, eventData.clickCount, clickType, MouseEventType.Down);
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

			if(GetMousePosition(out Vector2 pos))
				browserClient.SendMouseClickEvent(pos, eventData.clickCount, clickType, MouseEventType.Up);
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
				pos.x *= browserClient.width;
				pos.y *= browserClient.height;

				return true;
			}

			return false;
		}
	}
}