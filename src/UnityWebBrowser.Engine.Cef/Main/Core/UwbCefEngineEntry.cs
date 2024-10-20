// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

/// <summary>
///     <see cref="EngineEntryPoint" /> for the Cef engine
/// </summary>
internal class UwbCefEngineEntry : EngineEntryPoint
{
    private CefEngineControlsManager cefEngineControlsManager;

    protected override void EarlyInit(LaunchArguments launchArguments, string[] args)
    {
        cefEngineControlsManager = new CefEngineControlsManager(LoggerManagerFactory);
        cefEngineControlsManager.EarlyInit(launchArguments, args);
    }

    protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
    {
        
        cefEngineControlsManager.Init(ClientControlsActions, PopupManager);

        SetupIpc(cefEngineControlsManager, launchArguments);
        //Ready();

        //Calling run message loop will cause the main thread to lock (what we want)
        CefRuntime.RunMessageLoop();

        //If the message loop quits
        Dispose();
        Environment.Exit(0);
    }

    #region Destroy

    protected override void ReleaseResources()
    {
        cefEngineControlsManager?.Dispose();
        base.ReleaseResources();
    }

    #endregion
}