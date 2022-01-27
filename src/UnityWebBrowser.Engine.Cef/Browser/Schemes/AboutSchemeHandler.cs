using System.IO;
using System.Text;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Schemes;

public class AboutSchemeHandler : CefSchemeHandlerFactory
{
    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    {
        const string html = "<p>Hello World!</p>";
        byte[] bytes = new byte[html.Length];
        int size = Encoding.UTF8.GetBytes(html, bytes);
        
        MemoryStream stream = new MemoryStream();
        stream.Write(bytes, 0, size);

        SteamCefResourceHandler resourceHandler = new SteamCefResourceHandler(stream: stream, autoDisposeStream: true);
        return resourceHandler;
    }
}