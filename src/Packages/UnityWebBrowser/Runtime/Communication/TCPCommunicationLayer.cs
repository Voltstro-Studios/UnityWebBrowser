// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Net;
using UnityEngine;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;

namespace VoltstroStudios.UnityWebBrowser.Communication
{
    /// <summary>
    ///     In-Built TCP layer that uses VoltRpc's <see cref="TCPClient" /> and <see cref="TCPHost" />
    /// </summary>
    [CreateAssetMenu(fileName = "TCP Communication Layer", menuName = "UWB/TCP Communication Layer")]
    public sealed class TCPCommunicationLayer : CommunicationLayer
    {
        /// <summary>
        ///     The in port to communicate on
        /// </summary>
        [Header("TCP Settings")] [Range(1024, 65353)] [Tooltip("The in port to communicate on")]
        public int inPort = 5555;

        /// <summary>
        ///     The out port to communicate on
        /// </summary>
        [Range(1024, 65353)] [Tooltip("The out port to communicate on")]
        public int outPort = 5556;

        /// <summary>
        ///     How long the client keeps trying to reach the engine before giving up, in milliseconds
        /// </summary>
        /// <remarks>
        ///     This was previously hardcoded to <see cref="int.MaxValue" />, i.e. roughly 24 days of
        ///     retrying. If the engine process is already gone the connect loop therefore never
        ///     terminates, and because a Unity domain reload has to tear the client down, the reload
        ///     blocks on it - the editor hangs on "Reloading Domain (busy for ...)" indefinitely
        ///     rather than reporting a failed connection.
        /// </remarks>
        [Range(1000, 600000)] [Tooltip("Give up connecting to the engine after this long (ms)")]
        public int connectionTimeout = 30000;

        public override Client CreateClient()
        {
            IPEndPoint ipEndPoint = new(IPAddress.Loopback, inPort);
            return new TCPClient(ipEndPoint, connectionTimeout);
        }

        public override Host CreateHost()
        {
            IPEndPoint ipEndPoint = new(IPAddress.Loopback, outPort);
            return new TCPHost(ipEndPoint, int.MaxValue, int.MaxValue);
        }

        public override void GetIpcSettings(out object outLocation, out object inLocation, out string layerName)
        {
            outLocation = outPort;
            inLocation = inPort;
            layerName = "TCP";
        }
    }
}