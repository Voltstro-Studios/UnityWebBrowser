// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.Communication;

namespace VoltstroStudios.UnityWebBrowser.Shared.Communications;

/// <summary>
///     Layer for communications
/// </summary>
public interface ICommunicationLayer
{
    /// <summary>
    ///     Create a new <see cref="Client" />
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Client CreateClient(string location);

    /// <summary>
    ///     Create a new <see cref="Host" />
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Host CreateHost(string location);
}