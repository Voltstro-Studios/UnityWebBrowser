using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
    public class UwbCefContextMenuHandler : CefContextMenuHandler
    {
        protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters, CefMenuModel model,
            CefRunContextMenuCallback callback)
        {
            return true;
        }
    }
}