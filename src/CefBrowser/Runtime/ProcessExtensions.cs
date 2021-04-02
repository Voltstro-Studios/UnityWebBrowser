// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See https://github.com/dotnet/extensions/blob/ffb7c20fb22a31ac31d3a836a8455655867e8e16/LICENSE.txt for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace UnityWebBrowser
{
    public static class ProcessExtensions
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        public static void KillTree(this Process process)
        {
            process.KillTree(DefaultTimeout);
        }

        public static void KillTree(this Process process, TimeSpan timeout)
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
                HashSet<int> children = new HashSet<int>();
                GetAllChildIdsUnix(process.Id, children, timeout);
                foreach (int childId in children)
                {
                    KillProcessUnix(childId, timeout);
                }
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
	        using StringReader reader = new StringReader(stdout);
			while (true)
			{
				string text = reader.ReadLine();
				if (text == null)
				{
					return;
				}

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

        private static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
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
            {
                stdout = process.StandardOutput.ReadToEnd();
            }
            else
            {
                process.Kill();
            }

            return process.ExitCode;
        }
    }
}
