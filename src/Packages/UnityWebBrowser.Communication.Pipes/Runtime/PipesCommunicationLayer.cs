// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityEngine;
using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;

namespace VoltstroStudios.UnityWebBrowser.Communication.Pipes
{
    /// <summary>
    ///     <see cref="CommunicationLayer" /> using <see cref="PipesHost" /> and <see cref="PipesClient" />
    /// </summary>
    [CreateAssetMenu(fileName = "Pipes Communication Layer", menuName = "UWB/Pipes Communication Layer")]
    public sealed class PipesCommunicationLayer : CommunicationLayer
    {
        /// <summary>
        ///     What pipe name to communicate in
        /// </summary>
        public string inPipeName = "UnityWebBrowserIn";

        /// <summary>
        ///     What pipe name to communicate out
        /// </summary>
        public string outPipeName = "UnityWebBrowserOut";

        public override Client CreateClient()
        {
            return new PipesClient(inPipeName);
        }

        public override Host CreateHost()
        {
            return new PipesHost(outPipeName);
        }

        public override void GetIpcSettings(out object outLocation, out object inLocation, out string layerName)
        {
            outLocation = outPipeName;
            inLocation = inPipeName;
            layerName = "Pipes";
        }
    }
}