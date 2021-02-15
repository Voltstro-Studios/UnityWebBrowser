using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WebBrowser : MonoBehaviour
{
	private WebBrowserClient browserClient = new WebBrowserClient();

	private void Start()
	{
		browserClient.Init();
		GetComponent<MeshRenderer>().material.mainTexture = browserClient.BrowserTexture;
		browserClient.Update().Forget();
    }

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}