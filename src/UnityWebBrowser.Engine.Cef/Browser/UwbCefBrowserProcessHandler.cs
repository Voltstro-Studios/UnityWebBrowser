using System;
using UnityWebBrowser.Engine.Cef.Browser.Schemes;
using UnityWebBrowser.Engine.Shared.Pages;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

public class UwbCefBrowserProcessHandler : CefBrowserProcessHandler, IDisposable
{
    private readonly PageResourceScheme aboutPage;

    public UwbCefBrowserProcessHandler()
    {
        aboutPage = new PageResourceScheme(PagesHandler.GetAboutPageCode());
    }

    public void Dispose()
    {
        aboutPage?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void OnContextInitialized()
    {
        CefRuntime.RegisterSchemeHandlerFactory("uwb", "about", aboutPage);
    }
}