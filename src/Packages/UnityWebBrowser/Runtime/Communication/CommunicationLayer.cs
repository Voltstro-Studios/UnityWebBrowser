// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using JetBrains.Annotations;
using UnityEngine;
using VoltRpc.Communication;

namespace VoltstroStudios.UnityWebBrowser.Communication
{
    /// <summary>
    ///     Base class that communication layers need to implement.
    ///     <para>
    ///         The communication layer is what is responsible for providing what <see cref="Client" /> and
    ///         <see cref="Host" /> to use.
    ///     </para>
    /// </summary>
    public abstract class CommunicationLayer : ScriptableObject
    {
        /// <summary>
        ///     Timeout time for connection
        /// </summary>
        public int connectionTimeout = 7000;

        /// <summary>
        ///     Is this <see cref="CommunicationLayer" /> in use?
        /// </summary>
        internal bool IsInUse { get; set; }

        /// <summary>
        ///     Called when the <see cref="Client" /> needs to be created
        /// </summary>
        /// <returns></returns>
        public abstract Client CreateClient();

        /// <summary>
        ///     Called when the <see cref="Host" /> needs to be created
        /// </summary>
        /// <returns></returns>
        public abstract Host CreateHost();

        /// <summary>
        ///     Gets all settings needed for the IPC
        /// </summary>
        /// <param name="outLocation">
        ///     The "location" (whether that be a pipe name or port) that will allow communication from the
        ///     "outside" (The engine.)
        /// </param>
        /// <param name="inLocation">
        ///     The "location" (whether that be a pipe name or port) that will allow the client to
        ///     communication to the engine.
        /// </param>
        /// <param name="assemblyLocation">
        ///     The location of the assembly that the engine needs to load.
        ///     <para>To provide the location of an assembly <see cref="System.Reflection.Assembly.Location" /> can be used.</para>
        ///     <para>This can be null to say that no additional assembly is needed.</para>
        /// </param>
        public abstract void GetIpcSettings(out object outLocation, out object inLocation,
            [CanBeNull] out string assemblyLocation);
    }
}