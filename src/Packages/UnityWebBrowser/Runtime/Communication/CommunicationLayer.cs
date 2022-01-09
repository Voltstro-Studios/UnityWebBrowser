using UnityEngine;
using VoltRpc.Communication;

namespace UnityWebBrowser.Communication
{
    public abstract class CommunicationLayer : ScriptableObject
    {
        public int connectionTimeout = 7000;

        /// <summary>
        ///     Is this <see cref="CommunicationLayer" /> in use?
        /// </summary>
        internal bool IsInUse { get; set; }

        public abstract Client CreateClient();

        public abstract Host CreateHost();

        public abstract void GetIpcSettings(out object outLocation, out object inLocation, out string assemblyLocation);
    }
}