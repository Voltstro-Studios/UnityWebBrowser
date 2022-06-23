using System;
using System.Collections.Generic;
using System.Linq;
using UnityWebBrowser.Events;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared.Popups;

namespace UnityWebBrowser.Core.Popups
{
    internal class WebBrowserPopupService : IPopupEngineControls, IPopupClientControls
    {
        public WebBrowserPopupService(WebBrowserCommunicationsManager communicationsManager)
        {
            this.communicationsManager = communicationsManager;
            popups = new List<WebBrowserPopupInfo>();
            clientControls = new PopupClientControls(communicationsManager.ipcClient);
            onPopupCreated = communicationsManager.client.InvokeOnPopup;
            logger = communicationsManager.logger;
        }
        
        private readonly List<WebBrowserPopupInfo> popups;
        private readonly OnPopup onPopupCreated;
        private readonly IWebBrowserLogger logger;
        private readonly WebBrowserCommunicationsManager communicationsManager;
        private readonly PopupClientControls clientControls;

        #region Engine

        public void OnPopup(Guid guid)
        {
            logger.Debug($"Got popup {guid}");
            WebBrowserPopupInfo popupInfo = new WebBrowserPopupInfo(guid, this);
            popups.Add(popupInfo);
            
            onPopupCreated.Invoke(popupInfo);
        }

        public void OnPopupDestroy(Guid guid)
        {
            logger.Debug($"Destroy popup {guid}");
            WebBrowserPopupInfo popupInfo = popups.Single(x => x.PopupGuid == guid);
            popupInfo.DisposeNoSend();
            popups.Remove(popupInfo);
        }

        #endregion

        #region Client

        public void PopupClose(Guid guid)
        {
            communicationsManager.ExecuteTask(() => clientControls.PopupClose(guid));
        }

        public void PopupExecuteJs(Guid guid, string js)
        {
            communicationsManager.ExecuteTask(() => clientControls.PopupExecuteJs(guid, js));
        }

        #endregion
    }
}