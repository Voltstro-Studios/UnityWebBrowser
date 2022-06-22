using System;
using UnityWebBrowser.Shared.Popups;

namespace UnityWebBrowser.Core.Popups
{
    public class WebBrowserPopupInfo : EnginePopupInfo
    {
        internal WebBrowserPopupInfo(Guid guid)
            : base(guid)
        {
            IsValid = true;
        }
        
        public bool IsValid { get; private set; }

        public override void Dispose()
        {
            DisposeNoSend();
            base.Dispose();
        }

        internal void DisposeNoSend()
        {
            IsValid = false;
        }
    }
}