using UnityEngine;
using VoltRpc.Communication;

namespace UnityWebBrowser.Communication
{
    public abstract class CommunicationLayer : ScriptableObject
    {
        /// <summary>
        ///     Is this <see cref="CommunicationLayer"/> in use?
        /// </summary>
        internal bool IsInUse { get; set; }
        
        public int connectionTimeout = 7000;
        
        public abstract Client CreateClient();

        public abstract Host CreateHost();

        public abstract void GetIpcSettings(out object outLocation, out object inLocation);
    }
}
