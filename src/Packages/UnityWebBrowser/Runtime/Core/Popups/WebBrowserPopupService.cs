using System;
using System.Collections.Generic;
using System.Linq;
using UnityWebBrowser.Events;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared.Popups;

namespace UnityWebBrowser.Core.Popups
{
    public class WebBrowserPopupService : IPopupEngineControls
    {
        public WebBrowserPopupService(OnPopup onPopupCreated, IWebBrowserLogger logger)
        {
            popups = new List<WebBrowserPopupInfo>();
            this.onPopupCreated = onPopupCreated;
            this.logger = logger;
        }
        
        private readonly List<WebBrowserPopupInfo> popups;
        private readonly OnPopup onPopupCreated;
        private readonly IWebBrowserLogger logger;

        public void OnPopup(Guid guid)
        {
            logger.Debug($"Got popup {guid}");
            popups.Add(new WebBrowserPopupInfo(guid));
        }

        public void OnPopupDestroy(Guid guid)
        {
            logger.Debug($"Destroy popup {guid}");
            WebBrowserPopupInfo popupInfo = popups.Single(x => x.PopupGuid == guid);
            popupInfo.DisposeNoSend();
            popups.Remove(popupInfo);
        }
    }
}