// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using VoltRpc.Communication;
using VoltRpc.Logging;

namespace VoltstroStudios.UnityWebBrowser.Communication.Pipes.Base
{
    /// <summary>
    ///     <see cref="Host" /> using named pipes
    /// </summary>
    public sealed class PipesHost : Host
    {
        private readonly string pipeName;

        /// <summary>
        ///     Creates a new <see cref="PipesHost" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesHost(string pipeName, int bufferSize)
            : this(pipeName, null, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesHost" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="logger">The <see cref="ILogger" /> to use</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesHost(string pipeName, ILogger logger = null, int bufferSize = DefaultBufferSize)
            : base(logger, bufferSize)
        {
            this.pipeName = pipeName;
        }

        /// <inheritdoc />
        public override void StartListening()
        {
            CheckDispose();

            Logger.Debug("Named Pipes host now listening...");

            IsRunning = true;
            while (IsRunning)
                try
                {
                    if (ConnectionCount >= MaxConnectionsCount)
                        continue;

                    NamedPipeServerStream serverStream = new(pipeName, PipeDirection.InOut);
                    serverStream.WaitForConnection();
                    ConnectionCount++;
                    _ = Task.Run(() => HandleClient(serverStream));
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occured while handling incoming pipes connections! {ex}");
                }
        }

        private Task HandleClient(NamedPipeServerStream stream)
        {
            Logger.Debug("Accepted client...");
            ProcessRequest(stream, stream);
            stream.Dispose();
            Logger.Debug("Client disconnected.");
            ConnectionCount--;
            return Task.CompletedTask;
        }
    }
}