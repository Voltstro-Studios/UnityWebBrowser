// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Schemes;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Pages;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

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