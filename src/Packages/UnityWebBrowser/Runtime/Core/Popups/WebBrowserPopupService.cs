// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using VoltstroStudios.UnityWebBrowser.Events;
using VoltstroStudios.UnityWebBrowser.Logging;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;

namespace VoltstroStudios.UnityWebBrowser.Core.Popups
{
    internal class WebBrowserPopupService : IPopupEngineControls, IPopupClientControls
    {
        private readonly PopupClientControls clientControls;
        private readonly WebBrowserCommunicationsManager communicationsManager;
        private readonly IWebBrowserLogger logger;
        private readonly OnPopup onPopupCreated;

        private readonly List<WebBrowserPopupInfo> popups;

        public WebBrowserPopupService(WebBrowserCommunicationsManager communicationsManager)
        {
            this.communicationsManager = communicationsManager;
            popups = new List<WebBrowserPopupInfo>();
            clientControls = new PopupClientControls(communicationsManager.ipcClient);
            onPopupCreated = communicationsManager.client.InvokeOnPopup;
            logger = communicationsManager.logger;
        }

        #region Engine

        public void OnPopup(Guid guid)
        {
            logger.Debug($"Got popup {guid}");
            WebBrowserPopupInfo popupInfo = new(guid, this);
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