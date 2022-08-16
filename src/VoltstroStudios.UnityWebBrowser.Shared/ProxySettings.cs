// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Shared;

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