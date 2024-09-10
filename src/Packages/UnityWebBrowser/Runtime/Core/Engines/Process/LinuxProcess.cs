// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#if UNITY_STANDALONE_LINUX

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VoltstroStudios.UnityWebBrowser.Logging;

namespace VoltstroStudios.UnityWebBrowser.Core.Engines.Process
{
    internal sealed class LinuxProcess : IProcess
    {
        private readonly System.Diagnostics.Process process;
        private readonly IWebBrowserLogger logger;

        public LinuxProcess(IWebBrowserLogger logger)
        {
            process = new System.Diagnostics.Process();
            this.logger = logger;
        }

        public bool HasExited => process.HasExited;
        public int ExitCode => process.ExitCode;

        public void StartProcess(string executable, string workingDir, string arguments, DataReceivedEventHandler onLogEvent,
            DataReceivedEventHandler onErrorLogEvent)
        {
            ProcessStartInfo startInfo = new(executable, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDir
            };

            process.StartInfo = startInfo;
            process.OutputDataReceived += onLogEvent;
            process.ErrorDataReceived += onErrorLogEvent;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void KillProcess()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            HashSet<int> children = new();
            GetAllChildIdsUnix(process.Id, children, timeout);
            foreach (int childId in children) KillProcessUnix(childId, timeout);
            KillProcessUnix(process.Id, timeout);
        }
        
        public void Dispose()
        {
            process.Dispose();
        }
        
        private static void GetAllChildIdsUnix(int parentId, ISet<int> children, TimeSpan timeout)
        {
            int exitCode = RunProcessAndWaitForExit(
                "pgrep",
                $"-P {parentId}",
                timeout,
                out string stdout);

            if (exitCode != 0 || string.IsNullOrEmpty(stdout)) return;
            using StringReader reader = new(stdout);
            while (true)
            {
                string text = reader.ReadLine();
                if (text == null) return;

                if (!int.TryParse(text, out int id)) continue;

                children.Add(id);
                // Recursively get the children
                GetAllChildIdsUnix(id, children, timeout);
            }
        }

        private static void KillProcessUnix(int processId, TimeSpan timeout)
        {
            RunProcessAndWaitForExit(
                "kill",
                $"-TERM {processId}",
                timeout,
                out string _);
        }

        private static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout,
            out string stdout)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);

            stdout = null;
            if (process.WaitForExit((int)timeout.TotalMilliseconds))
                stdout = process.StandardOutput.ReadToEnd();
            else
                process.Kill();

            return process.ExitCode;
        }
    }
}

#endif