using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WebBrowser : MonoBehaviour
{
	public WebBrowserClient browserClient = new WebBrowserClient();

	private void Start()
	{
		browserClient.Init();
		GetComponent<MeshRenderer>().material.mainTexture = browserClient.BrowserTexture;
    }

	private void OnDestroy()
	{
		browserClient.Shutdown();
	}
}