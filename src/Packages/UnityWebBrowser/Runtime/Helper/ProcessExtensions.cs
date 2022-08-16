// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace VoltstroStudios.UnityWebBrowser.Helper
{
    internal static class ProcessExtensions
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        public static void KillTree(this Process process)
        {
            process.KillTree(DefaultTimeout);
        }

        private static void KillTree(this Process process, TimeSpan timeout)
        {
            if (IsWindows)
            {
                RunProcessAndWaitForExit(
                    "taskkill",
                    $"/T /F /PID {process.Id}",
                    timeout,
                    out string _);
            }
            else
            {
                HashSet<int> children = new();
                GetAllChildIdsUnix(process.Id, children, timeout);
                foreach (int childId in children) KillProcessUnix(childId, timeout);
                KillProcessUnix(process.Id, timeout);
            }
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

            Process process = Process.Start(startInfo);

            stdout = null;
            if (process.WaitForExit((int)timeout.TotalMilliseconds))
                stdout = process.StandardOutput.ReadToEnd();
            else
                process.Kill();

            return process.ExitCode;
        }
    }
}