// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityEngine;
using UnityEngine.UI;
using VoltstroStudios.UnityWebBrowser.Core;

namespace VoltstroStudios.UnityWebBrowser
{
    /// <summary>
    ///     Included UI controller for UWB.
    ///     <para>Handles the URL input field.</para>
    /// </summary>
    [AddComponentMenu("UWB/Web Browser UI Controls")]
    [HelpURL("https://github.com/Voltstro-Studios/UnityWebBrowser")]
    [RequireComponent(typeof(BaseUwbClientManager))]
    [DisallowMultipleComponent]
    public sealed class WebBrowserUIControls : MonoBehaviour
    {
        /// <summary>
        ///     The <see cref="InputField" /> for the URL
        /// </summary>
        [Tooltip("The input field for the URL")]
        public InputField inputField;

        private BaseUwbClientManager webBrowserUi;

        private void Start()
        {
            if (inputField == null)
                throw new NullReferenceException("Input field is null!");

            webBrowserUi = GetComponent<BaseUwbClientManager>();
            if (webBrowserUi == null)
                throw new NullReferenceException("Web browser UI is null!");

            webBrowserUi.browserClient.OnUrlChanged += OnUrlChanged;
            inputField.text = webBrowserUi.browserClient.initialUrl;
        }

        private void OnDestroy()
        {
            webBrowserUi.browserClient.OnUrlChanged -= OnUrlChanged;
        }

        private void OnUrlChanged(string url)
        {
            inputField.text = url;
        }

        /// <summary>
        ///     Submits the input field's text to be navigated to
        /// </summary>
        public void SubmitInput()
        {
            if (webBrowserUi == null) return;

            if (!string.IsNullOrEmpty(inputField.text))
                webBrowserUi.NavigateUrl(inputField.text);
            else
                throw new NullReferenceException("The web browser UI is null!");
        }
    }
}