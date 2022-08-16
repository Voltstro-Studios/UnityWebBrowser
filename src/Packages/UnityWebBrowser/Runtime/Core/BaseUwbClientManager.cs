// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityEngine;

namespace VoltstroStudios.UnityWebBrowser.Core
{
    /// <summary>
    ///     Base for all other systems that want to build custom rendering with UWB.
    ///     <para>Provides controls that can be used with Unity events (such as OnClick).</para>
    ///     <para>For other controls and events, use the <see cref="browserClient" />.</para>
    ///     <para>
    ///         For those who want to implement this base, DO NOT USE <c>Start()</c>, <c>FixedUpdate</c> or <c>OnDestroy</c>,
    ///         instead override <see cref="OnStart" />, <see cref="OnFixedUpdate" /> and <see cref="OnDestroyed" />.
    ///     </para>
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseUwbClientManager : MonoBehaviour
    {
        /// <summary>
        ///     The <see cref="WebBrowserClient" />, what handles the communication between the UWB engine and Unity
        /// </summary>
        [Tooltip("The browser client, what handles the communication between the UWB engine and Unity")]
        public WebBrowserClient browserClient = new();

        private void Start()
        {
            //Start the browser client
            browserClient.Init();

            OnStart();
        }

        private void Update()
        {
            browserClient.UpdateFps();
        }

        private void FixedUpdate()
        {
            browserClient.LoadTextureData();
            OnFixedUpdate();
        }

        private void OnDestroy()
        {
            browserClient.Dispose();
            OnDestroyed();
        }

        /// <summary>
        ///     Override this instead of using <see cref="Start" />
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        ///     Override this instead of using <see cref="FixedUpdate" />
        /// </summary>
        protected virtual void OnFixedUpdate()
        {
        }

        /// <summary>
        ///     Override this instead of using <see cref="OnDestroy" />
        /// </summary>
        protected virtual void OnDestroyed()
        {
        }

        #region Client Methods

        /// <summary>
        ///     Makes the browser go back a page
        /// </summary>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void GoBack()
        {
            browserClient.GoBack();
        }

        /// <summary>
        ///     Make the browser go forward a page
        /// </summary>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void GoForward()
        {
            browserClient.GoForward();
        }

        /// <summary>
        ///     Makes the browser go to a url
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void NavigateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            browserClient.LoadUrl(url);
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void Refresh()
        {
            browserClient.Refresh();
        }

        /// <summary>
        ///     Loads HTML code
        /// </summary>
        /// <param name="html"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void LoadHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                throw new ArgumentNullException(nameof(html));

            browserClient.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS
        /// </summary>
        /// <param name="js"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        public void ExecuteJs(string js)
        {
            if (string.IsNullOrWhiteSpace(js))
                throw new ArgumentNullException(nameof(js));

            browserClient.ExecuteJs(js);
        }

        #endregion
    }
}