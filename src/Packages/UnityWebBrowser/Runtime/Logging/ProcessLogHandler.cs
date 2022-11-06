// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Shared;

namespace VoltstroStudios.UnityWebBrowser.Logging
{
    /// <summary>
    ///     Handles UWB logs
    /// </summary>
    [Preserve]
    public class ProcessLogHandler
    {
        private readonly IWebBrowserLogger logger;

        internal ProcessLogHandler(WebBrowserClient client)
        {
            logger = client.logger;
        }

        public event Action<string> OnProcessOutputLog;

        public event Action<string> OnProcessErrorLog;

        internal void HandleErrorProcessLog(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            OnProcessErrorLog?.Invoke(e.Data);
        }

        internal void HandleOutputProcessLog(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            try
            {
                JsonLogStructure logStructure = ReadJsonLog(e.Data);

                if (logStructure.Level is LogSeverity.Debug or LogSeverity.Info)
                    logger.Debug(logStructure.Message);
                else if (logStructure.Level == LogSeverity.Warn)
                    logger.Warn(logStructure.Message);
                else if (logStructure.Level == LogSeverity.Error)
                    logger.Error($"{logStructure.Message}\n{logStructure.Exception}");
                else if (logStructure.Level == LogSeverity.Fatal)
                    logger.Error($"{logStructure.Message}\n{logStructure.Exception}");
            }
            catch (Exception ex)
            {
                logger.Error($"An error occured with processing a log event from the UWB engine!\n\nRaw Log Message:\n{e.Data}\n\nException:\n{ex}");
            }

            OnProcessOutputLog?.Invoke(e.Data);
        }

        /// <summary>
        ///     Reads json and deserializes it as <see cref="JsonLogStructure" />
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static JsonLogStructure ReadJsonLog(string json)
        {
            JsonLogStructure logStructure = JsonConvert.DeserializeObject<JsonLogStructure>(json);
            return logStructure;
        }
    }
}