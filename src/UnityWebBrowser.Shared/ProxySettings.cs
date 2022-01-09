using System;

namespace UnityWebBrowser.Shared;

/// <summary>
///     Settings for the proxy
/// </summary>
[Serializable]
public struct ProxySettings
{
    /// <summary>
    ///     The username to use for auth with the proxy
    /// </summary>
    public string Username;

    /// <summary>
    ///     The password to use for auth with the proxy
    /// </summary>
    public string Password;

    /// <summary>
    ///     Enable or disable the proxy server
    /// </summary>
    public bool ProxyServer;

    public ProxySettings(string username, string password, bool proxyServer)
    {
        Username = username;
        Password = password;
        ProxyServer = proxyServer;
    }
}