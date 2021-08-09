namespace UnityWebBrowser.Logging
{
    /// <summary>
    ///     Interface for the web browser's logger.
    ///     <para>Implement </para>
    /// </summary>
    public interface IWebBrowserLogger
    {
        public void Debug(object message);

        public void Warn(object message);

        public void Error(object message);
    }
}