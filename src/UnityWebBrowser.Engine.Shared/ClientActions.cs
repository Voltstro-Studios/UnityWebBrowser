using System;
using UnityWebBrowser.Shared;
using VoltRpc.Communication;
using VoltRpc.Proxy.Generated;

#nullable enable
namespace UnityWebBrowser.Engine.Shared
{
    /// <summary>
    ///     This is a wrapper around <see cref="IClient"/>. It checks if we are connected first before firing an event.
    ///     <para>The reason why we do the check here is that VoltRpc will throw an exception, rather then not do anything.</para>
    /// </summary>
    public class ClientActions : IClient
    {
        private Client? client;
        private IClient? clientActions;

        internal void SetIpcClient(Client ipcClient)
        {
            client = ipcClient ?? throw new NullReferenceException();
            clientActions = new ClientProxy(client);
        }
        
        public void UrlChange(string url)
        {
            if(client is { IsConnected: true })
                clientActions?.UrlChange(url);
        }

        public void LoadStart(string url)
        {
            if(client is { IsConnected: true })
                clientActions?.LoadStart(url);
        }

        public void LoadFinish(string url)
        {
            if(client is { IsConnected: true })
                clientActions?.LoadFinish(url);
        }

        public void TitleChange(string title)
        {
            if(client is { IsConnected: true })
                clientActions?.TitleChange(title);
        }

        public void ProgressChange(double progress)
        {
            if(client is { IsConnected: true })
                clientActions?.ProgressChange(progress);
        }

        public void Fullscreen(bool fullScreen)
        {
            if(client is { IsConnected: true })
                clientActions?.Fullscreen(fullScreen);
        }
    }
}