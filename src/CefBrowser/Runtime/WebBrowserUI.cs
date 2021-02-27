using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WebBrowserUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public RawImage image;
	public WebBrowserClient browserClient = new WebBrowserClient();

	private Coroutine pointerKeyboardHandler;

	private static KeyCode[] keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

	private void Start()
	{
		browserClient.Init();
		StartCoroutine(browserClient.Start());
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

		browserClient.SetKeyboardData("", new int[0], new int[0]);
	}

	private IEnumerator HandlerPointerAndKeyboardData()
	{
		while (Application.isPlaying)
		{
			List<int> keysDown = new List<int>();
			List<int> keysUp = new List<int>();

			foreach (KeyCode key in keymap)
			{
				if(Input.GetKeyDown(key))
					keysDown.Add((int)key);
				if(Input.GetKeyUp(key))
					keysUp.Add((int)key);
			}

			browserClient.SetKeyboardData(Input.inputString, keysDown.ToArray(), keysUp.ToArray());

			Vector2 pos = GetScreenCoords();

			browserClient.SetMousePosData((int)pos.x, (int)pos.y);

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