using System;
using System.Linq;
using UnityWebBrowser.Engine.Shared.Core;
using UnityWebBrowser.Engine.Shared.Core.Logging;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

/// <summary>
///     <see cref="EngineEntryPoint" /> for the Cef engine
/// </summary>
public class UwbCefEngineEntry : EngineEntryPoint
{
    private CefEngineManager cefEngineManager;

    protected override bool ShouldInitLogger(LaunchArguments launchArguments, string[] args)
    {
        return !args.Any(arg => arg.StartsWith("--type="));
    }

    protected override void EarlyInit(LaunchArguments launchArguments, string[] args)
    {
        cefEngineManager = new CefEngineManager();
        cefEngineManager.EarlyInit(launchArguments, args);
    }

    protected override void EntryPoint(LaunchArguments launchArguments, string[] args)
    {
        cefEngineManager.Init(ClientActions);

        SetupIpc(cefEngineManager, launchArguments);
        Ready();

        //Calling run message loop will cause the main thread to lock (what we want)
        CefRuntime.RunMessageLoop();

        //If the message loop quits
        Logger.Debug($"{CefLoggerWrapper.FullCefMessageTag} Message loop quit.");
        Dispose();
        Environment.Exit(0);
    }

    #region Destroy

    protected override void ReleaseResources()
    {
        cefEngineManager?.Dispose();
        base.ReleaseResources();
    }

    #endregion
}