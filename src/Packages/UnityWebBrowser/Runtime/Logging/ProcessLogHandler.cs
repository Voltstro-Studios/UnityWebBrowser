using System;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Logging
{
    internal class ProcessLogHandler
    {
        private readonly IWebBrowserLogger logger;

        public ProcessLogHandler(WebBrowserClient client)
        {
            logger = client.logger;
        }

        public void HandleProcessLog(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            try
            {
                JsonLogStructure logStructure = ReadJsonLog(e.Data);

                if (logStructure.Level == LogSeverity.Debug || logStructure.Level == LogSeverity.Info)
                    logger.Debug(logStructure.Message);
                else if(logStructure.Level == LogSeverity.Warn)
                    logger.Warn(logStructure.Message);
                else if (logStructure.Level == LogSeverity.Error)
                    logger.Error(logStructure.Message);
                else if(logStructure.Level == LogSeverity.Error && logStructure.Exception != null)
                    logger.Error($"{logStructure.Exception}\n{logStructure.Exception}");
                else if(logStructure.Level == LogSeverity.Fatal)
                    logger.Error(logStructure.Message);
                else if(logStructure.Level == LogSeverity.Fatal && logStructure.Exception != null)
                    logger.Error(logStructure.Message);

            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        ///     Reads json and deserializes it as <see cref="JsonLogStructure"/>
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