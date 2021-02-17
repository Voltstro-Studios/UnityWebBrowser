using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebBrowserUI : MonoBehaviour
{
	public WebBrowserClient browserClient = new WebBrowserClient();

	private static KeyCode[] keymap = (KeyCode[])Enum.GetValues(typeof(KeyCode));

	private void Start()
	{
		browserClient.Init();
		StartCoroutine(browserClient.Start());
		GetComponent<RawImage>().texture = browserClient.BrowserTexture;
	}

	private void Update()
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
	}

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}