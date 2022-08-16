// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Net;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;

/// <summary>
///     Communication layer using <see cref="TCPClient" /> and <see cref="TCPHost" />.
/// </summary>
internal class TCPCommunicationLayer : ICommunicationLayer
{
    public Client CreateClient(string location)
    {
        if (int.TryParse(location, out int port)) return CreateTCPClient(port);

        throw new ArgumentOutOfRangeException(nameof(location));
    }

    public Host CreateHost(string location)
    {
        if (int.TryParse(location, out int port)) return CreateTCPHost(port);

        throw new ArgumentOutOfRangeException(nameof(location));
    }

    /// <summary>
    ///     Creates a new <see cref="TCPClient" />
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public Client CreateTCPClient(int port)
    {
        IPEndPoint ipEndPoint = new(IPAddress.Loopback, port);
        return new TCPClient(ipEndPoint);
    }

    /// <summary>
    ///     Creates a new <see cref="TCPHost" />
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public Host CreateTCPHost(int port)
    {
        IPEndPoint ipEndPoint = new(IPAddress.Loopback, port);
        return new TCPHost(ipEndPoint);
    }
}