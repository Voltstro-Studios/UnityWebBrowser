// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license.See the LICENSE.md file for more details.

using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;

internal class PipesCommunicationLayer : ICommunicationLayer
{
    public Client CreateClient(string location)
    {
        return new PipesClient(location);
    }

    public Host CreateHost(string location)
    {
        return new PipesHost(location);
    }
}