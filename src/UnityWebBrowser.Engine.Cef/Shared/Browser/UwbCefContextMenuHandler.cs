// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

public class UwbCefContextMenuHandler : CefContextMenuHandler
{
    protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters,
        CefMenuModel model,
        CefRunContextMenuCallback callback)
    {
        return true;
    }
}