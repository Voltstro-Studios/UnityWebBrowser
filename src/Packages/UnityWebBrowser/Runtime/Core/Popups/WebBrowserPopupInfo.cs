using System;
using System.Diagnostics;
using UnityWebBrowser.Shared.Popups;

namespace UnityWebBrowser.Core.Popups
{
    public class WebBrowserPopupInfo : EnginePopupInfo
    {
        internal WebBrowserPopupInfo(Guid guid, WebBrowserPopupService popupService)
            : base(guid)
        {
            IsValid = true;
            this.popupService = popupService;
        }

        private readonly WebBrowserPopupService popupService;
        
        /// <summary>
        ///     Is this <see cref="WebBrowserPopupInfo"/> still valid?
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        ///     Called when this <see cref="WebBrowserPopupInfo"/> is destroyed in anyway
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
            if(!IsValid)
                throw new ObjectDisposedException(nameof(WebBrowserPopupInfo), "This popup has already been disposed of!");
        }
    }
}