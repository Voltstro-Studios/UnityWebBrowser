using System;
using UnityEngine;

public class WebBrowser : MonoBehaviour
{
	private WebBrowserClient browserClient = new WebBrowserClient();

	private void Start()
    {
	    browserClient.Init();
    }

	private void FixedUpdate()
	{
		browserClient.FixedUpdate();
	}

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}