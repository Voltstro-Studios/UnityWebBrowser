using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

		private static readonly KeyCode[] Keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

		private void Start()
		{
			//Start the browser client
			browserClient.Init();
			StartCoroutine(browserClient.Start());

			image = GetComponent<RawImage>();
			image.texture = browserClient.BrowserTexture;
			image.uvRect = new Rect(0f, 0f, 1f, -1f);
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

				foreach (KeyCode key in Keymap)
				{
					if(Input.GetKeyDown(key))
						keysDown.Add((int)key);
					if(Input.GetKeyUp(key))
						keysUp.Add((int)key);
				}

				browserClient.SendKeyboardEvent(keysDown.ToArray(), keysUp.ToArray(), Input.inputString);
				if (GetMousePosition(out Vector2 pos))
				{
					browserClient.SendMouseMoveEvent((int)pos.x, (int)pos.y);

					//Mouse scroll
					float scroll = Input.GetAxis("Mouse ScrollWheel");
					scroll *= browserClient.BrowserTexture.height;

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
				browserClient.SendMouseClickEvent((int)pos.x, (int)pos.y, eventData.clickCount, clickType, MouseEventType.Down);
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
				browserClient.SendMouseClickEvent((int)pos.x, (int)pos.y, eventData.clickCount, clickType, MouseEventType.Up);
		}

		private bool GetMousePosition(out Vector2 pos)
		{
			if (WebBrowserUtils.GetScreenPointToLocalPositionDeltaOnImage(image, Input.mousePosition,
				out pos))
			{
				pos.x *= browserClient.width;
				pos.y *= browserClient.height;

				return true;
			}

			return false;
		}
	}
}