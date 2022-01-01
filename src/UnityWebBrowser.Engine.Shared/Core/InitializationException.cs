using System;

namespace UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     An error when something failed while initializing or de-initializing
/// </summary>
public class InitializationException : Exception
{
    public InitializationException()
    {
    }

    public InitializationException(string message)
        : base(message)
    {
    }

    public InitializationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}