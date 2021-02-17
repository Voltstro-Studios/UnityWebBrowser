using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebBrowserUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public WebBrowserClient browserClient = new WebBrowserClient();

	private Coroutine pointerKeyboardHandler;

	private static KeyCode[] keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

	private void Start()
	{
		browserClient.Init();
		StartCoroutine(browserClient.Start());
		GetComponent<RawImage>().texture = browserClient.BrowserTexture;
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

		browserClient.SetData("", new int[0], new int[0]);
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

			browserClient.SetData(Input.inputString, keysDown.ToArray(), keysUp.ToArray());

			yield return 0;
		}
	}
}