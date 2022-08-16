// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.Communication;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;

namespace VoltstroStudios.UnityWebBrowser.Communication.Pipes.Base
{
    public class EnginePipesCommunicationLayer : ICommunicationLayer
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
}