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