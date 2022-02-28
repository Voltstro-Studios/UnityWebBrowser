using System;

namespace UnityWebBrowser
{
    /// <summary>
    ///     An <see cref="Exception"/> when UWB is not ready
    /// </summary>
    public sealed class UwbIsNotReadyException : Exception
    {
        internal UwbIsNotReadyException(string message)
            : base(message)
        {
        }
    }
}