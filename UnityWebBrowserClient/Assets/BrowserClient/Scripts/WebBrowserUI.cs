using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebBrowserUI : MonoBehaviour
{
	public WebBrowserClient browserClient = new WebBrowserClient();

	private void Start()
	{
		browserClient.Init();
		browserClient.Start().Forget();
		GetComponent<RawImage>().texture = browserClient.BrowserTexture;
	}

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}