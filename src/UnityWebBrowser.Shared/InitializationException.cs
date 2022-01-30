using System;

namespace UnityWebBrowser.Shared;

/// <summary>
///     An error when something failed while initializing or de-initializing
/// </summary>
public class InitializationException : Exception
{
    /// <summary>
    ///     Creates a new <see cref="InitializationException"/>
    /// </summary>
    public InitializationException()
    {
    }

    /// <summary>
    ///     Creates a new <see cref="InitializationException"/>
    /// </summary>
    public InitializationException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Creates a new <see cref="InitializationException"/>
    /// </summary>
    public InitializationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}