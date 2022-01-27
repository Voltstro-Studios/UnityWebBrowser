using System;
using System.IO;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Schemes;

public sealed class PageResourceScheme : CefSchemeHandlerFactory, IDisposable
{
    private readonly SteamCefResourceHandler resourceHandler;
    
    public PageResourceScheme(Stream stream, string mimeType = SteamCefResourceHandler.DefaultMimeType)
    {
        resourceHandler = new SteamCefResourceHandler(mimeType, stream, true);
    }

    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    {
        return resourceHandler;
    }

    public void Dispose()
    {
        resourceHandler.Dispose();
    }
}