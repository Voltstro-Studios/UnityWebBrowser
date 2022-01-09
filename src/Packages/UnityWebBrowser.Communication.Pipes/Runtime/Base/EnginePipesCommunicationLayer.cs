using UnityWebBrowser.Shared.Communications;
using VoltRpc.Communication;

namespace UnityWebBrowser.Communication.Pipes.Base
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