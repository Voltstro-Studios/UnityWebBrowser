using System;
using System.IO;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Schemes;

public sealed class PageResourceScheme : CefSchemeHandlerFactory, IDisposable
{
    private readonly Stream stream;
    private readonly string mimeType;
    
    public PageResourceScheme(Stream stream, string mimeType = StreamCefResourceHandler.DefaultMimeType)
    {
        this.stream = stream;
        this.mimeType = mimeType;
    }

    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    {
        return new StreamCefResourceHandler(mimeType, stream, false);
    }

    public void Dispose()
    {
        stream.Dispose();
    }
}