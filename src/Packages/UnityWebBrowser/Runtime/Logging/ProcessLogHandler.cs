using System.Diagnostics;

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
            if(e.Data == null)
                return;
            
            if (e.Data.StartsWith("DEBUG "))
                logger.Debug(e.Data.Replace("DEBUG ", ""));
            else if (e.Data.StartsWith("INFO "))
                logger.Debug(e.Data.Replace("INFO ", ""));
            else if (e.Data.StartsWith("WARN "))
                logger.Warn(e.Data.Replace("WARN ", ""));
            else if (e.Data.StartsWith("ERROR "))
                logger.Error(e.Data.Replace("ERROR ", ""));
            else
                logger.Debug(e.Data);
        }
    }
}