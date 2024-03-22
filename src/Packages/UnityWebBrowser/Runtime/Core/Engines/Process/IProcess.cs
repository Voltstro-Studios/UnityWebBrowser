// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Diagnostics;

namespace VoltstroStudios.UnityWebBrowser.Core.Engines.Process
{
    internal interface IProcess : IDisposable
    {
        public void StartProcess(string executable, string workingDir, string arguments, DataReceivedEventHandler onLogEvent, DataReceivedEventHandler onErrorLogEvent);

        public void KillProcess();
        
        public bool HasExited { get; }
        
        public int ExitCode { get; }
    }
}