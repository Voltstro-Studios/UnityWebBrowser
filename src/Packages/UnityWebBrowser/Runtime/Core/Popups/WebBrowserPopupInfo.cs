// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Diagnostics;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;

namespace VoltstroStudios.UnityWebBrowser.Core.Popups
{
    public class WebBrowserPopupInfo : EnginePopupInfo
    {
        private readonly WebBrowserPopupService popupService;

        internal WebBrowserPopupInfo(Guid guid, WebBrowserPopupService popupService)
            : base(guid)
        {
            IsValid = true;
            this.popupService = popupService;
        }

        /// <summary>
        ///     Is this <see cref="WebBrowserPopupInfo" /> still valid?
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        ///     Called when this <see cref="WebBrowserPopupInfo" /> is destroyed in anyway
        /// </summary>
        public event Action OnDestroyed;

        /// <inheritdoc />
        public override void ExecuteJs(string js)
        {
            ThrowIfInvalid();

            popupService.PopupExecuteJs(PopupGuid, js);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            ThrowIfInvalid();

            popupService.PopupClose(PopupGuid);
            base.Dispose();
        }

        /// <summary>
        ///     Invalidates this popup without sending a request
        /// </summary>
        internal void DisposeNoSend()
        {
            IsValid = false;
            OnDestroyed?.Invoke();
        }

        [DebuggerStepThrough]
        private void ThrowIfInvalid()
        {
            if (!IsValid)
                throw new ObjectDisposedException(nameof(WebBrowserPopupInfo),
                    "This popup has already been disposed of!");
        }
    }
}