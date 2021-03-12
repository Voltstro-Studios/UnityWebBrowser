#if TMP_ENABLED

using System;
using TMPro;
using UnityEngine;

namespace UnityWebBrowser
{
	/// <summary>
	///		Allows for UI controls with the input
	/// </summary>
	[AddComponentMenu("Browser/Web Browser UI Controls (TMP)")]
	[HelpURL("https://github.com/Voltstro-Studios/UnityWebBrowser")]
    [RequireComponent(typeof(WebBrowserUI))]
    public class WebBrowserUIControlsTMP : MonoBehaviour
    {
	    public TMP_InputField inputField;

	    private WebBrowserUI webBrowserUi;

	    private void Start()
	    {
		    if (inputField == null)
			    throw new NullReferenceException("Input field is null!");

		    webBrowserUi = GetComponent<WebBrowserUI>();
		    if (webBrowserUi == null)
			    throw new NullReferenceException("Web browser UI is null!");

		    inputField.text = webBrowserUi.browserClient.initialUrl;
	    }

		/// <summary>
		///		Submits the input field's text to be navigated to
		/// </summary>
		public void SubmitInput()
		{
			if (webBrowserUi == null) return;

			if(!string.IsNullOrEmpty(inputField.text))
				webBrowserUi.NavigateUrl(inputField.text);
			else
				throw new NullReferenceException("The web browser UI is null!");
		}
    }
}

#endif