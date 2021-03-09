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
	public sealed class WebBrowserUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		///		The <see cref="WebBrowserClient"/>, what handles the communication between the CEF process and Unity
		/// </summary>
		[Tooltip("The browser client, what handles the communication between the CEF process and Unity")]
		public WebBrowserClient browserClient = new WebBrowserClient();

		private Coroutine pointerKeyboardHandler;

		private static readonly KeyCode[] Keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

		private void Start()
		{
			//Start the browser client
			browserClient.Init();
			StartCoroutine(browserClient.Start());

			RawImage image = GetComponent<RawImage>();
			image.texture = browserClient.BrowserTexture;
			image.uvRect = new Rect(0f, 0f, 1f, -1f);
		}

		private void OnDestroy()
		{
			browserClient.Shutdown();
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

				//Vector2 pos = GetScreenCoords();

				yield return 0;
			}
		}

		//TODO: CEF does it mouse point from top left origin, so we need to calculate that, not the center
		private Vector2 GetScreenCoords()
		{
			GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
			Vector3 mousepos = Input.mousePosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousepos, raycaster.eventCamera, out Vector2 localPos);

			return localPos;
		}
	}
}