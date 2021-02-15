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
		GetComponent<RawImage>().texture = browserClient.BrowserTexture;
		browserClient.Update().Forget();
	}

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}