// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Diagnostics;
using VoltstroStudios.UnityWebBrowser.Core.Engines.Process;
using VoltstroStudios.UnityWebBrowser.Helper;
using VoltstroStudios.UnityWebBrowser.Logging;

namespace VoltstroStudios.UnityWebBrowser.Core.Engines
{
    internal sealed class EngineProcess : IDisposable
    {
        private readonly IProcess processHandle;
        private readonly Engine engine;
        private readonly IWebBrowserLogger logger;
        
        public EngineProcess(Engine engine, IWebBrowserLogger logger)
        {
            processHandle = new WindowProcess();
            
            this.engine = engine;
            this.logger = logger;
        }

        public bool HasExited => processHandle.HasExited;

        public int ExitCode => processHandle.ExitCode;

        public void StartProcess(string arguments,  DataReceivedEventHandler onLogEvent, DataReceivedEventHandler onErrorLogEvent)
        {
            string engineFullProcessPath = WebBrowserUtils.GetBrowserEngineProcessPath(engine);
            string engineDirectory = WebBrowserUtils.GetBrowserEnginePath(engine);
            
            logger.Debug($"Process Path: '{engineFullProcessPath}'\nWorking: '{engineDirectory}'");
            logger.Debug($"Arguments: '{arguments}'");
            
            processHandle.StartProcess(engineFullProcessPath, engineDirectory, arguments, onLogEvent, onErrorLogEvent);
        }

        public void KillProcess()
        {
            processHandle.KillProcess();
        }
        
        public void Dispose()
        {
            processHandle.Dispose();
        }
    }
}