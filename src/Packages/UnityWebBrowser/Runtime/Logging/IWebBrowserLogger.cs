// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Logging
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