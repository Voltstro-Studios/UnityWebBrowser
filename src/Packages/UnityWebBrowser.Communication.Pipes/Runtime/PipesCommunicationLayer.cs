using System.Reflection;
using UnityEngine;
using UnityWebBrowser.Communication.Pipes.Base;
using VoltRpc.Communication;

namespace UnityWebBrowser.Communication.Pipes
{
    [CreateAssetMenu(fileName = "Pipes Communication Layer", menuName = "UWB/Pipes Communication Layer")]
    public class PipesCommunicationLayer : CommunicationLayer
    {
        public string inPipeName = "UnityWebBrowserIn";
        public string outPipeName = "UnityWebBrowserOut";
        
        public override Client CreateClient()
        {
            return new PipesClient(inPipeName, connectionTimeout, Client.DefaultBufferSize);
        }

        public override Host CreateHost()
        {
            return new PipesHost(outPipeName);
        }

        public override void GetIpcSettings(out object outLocation, out object inLocation, out string assemblyLocation)
        {
            outLocation = outPipeName;
            inLocation = inPipeName;
            assemblyLocation = typeof(EnginePipesCommunicationLayer).Assembly.Location;
        }
    }
}