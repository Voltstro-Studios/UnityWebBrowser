using System;
using System.Net;
using System.Threading;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events;
using UnityWebBrowser.Shared.ReadWriters;
using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Proxy.Generated;

namespace UnityWebBrowser
{
    internal class WebBrowserCommunicationsManager : IEngine, IClient, IDisposable
    {
        private readonly IEngine engineProxy;
        private readonly Client client;
        private readonly Host host;

        private readonly SynchronizationContext unityThread;
        private readonly IWebBrowserLogger logger;

        public bool IsConnected => client.IsConnected;

        public WebBrowserCommunicationsManager(WebBrowserIpcSettings ipcSettings, IWebBrowserLogger logger)
        {
            unityThread = SynchronizationContext.Current;
            this.logger = logger;

            if (ipcSettings.preferPipes)
            {
                logger.Debug("Using pipes communication...");

                host = new PipesHost(ipcSettings.inPipeName);
                client = new PipesClient(ipcSettings.outPipeName, ipcSettings.connectionTimeout,
                    Client.DefaultBufferSize);
            }
            else
            {
                logger.Debug("Using TCP communication...");

                host = new TCPHost(new IPEndPoint(IPAddress.Loopback, (int)ipcSettings.inPort));
                client = new TCPClient(new IPEndPoint(IPAddress.Loopback, (int)ipcSettings.outPort));
            }

            ReadWriterUtils.AddTypeReadWriters(host.TypeReaderWriterManager);
            host.AddService<IClient>(this);

            ReadWriterUtils.AddTypeReadWriters(client.TypeReaderWriterManager);
            client.AddService<IEngine>();
            engineProxy = new EngineProxy(client);
        }

        public void Connect()
        {
            client.Connect();
        }

        public void Listen()
        {
            host.StartListening();
        }

        public byte[] GetPixels()
        {
            return engineProxy.GetPixels();
        }

        public void Shutdown()
        {
            engineProxy.Shutdown();
        }

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            engineProxy.SendKeyboardEvent(keyboardEvent);
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            engineProxy.SendMouseMoveEvent(mouseMoveEvent);
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            engineProxy.SendMouseClickEvent(mouseClickEvent);
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            engineProxy.SendMouseScrollEvent(mouseScrollEvent);
        }

        public void GoForward()
        {
            engineProxy.GoForward();
        }

        public void GoBack()
        {
            engineProxy.GoBack();
        }

        public void Refresh()
        {
            engineProxy.Refresh();
        }

        public void LoadUrl(string url)
        {
            engineProxy.LoadUrl(url);
        }

        public void LoadHtml(string html)
        {
            engineProxy.LoadHtml(html);
        }

        public void ExecuteJs(string js)
        {
            engineProxy.ExecuteJs(js);
        }

        public void Dispose()
        {
            Shutdown();
            client.Dispose();
            host.Dispose();
        }

        public event Action<string> OnUrlChanged;
        public event Action<string> OnLoadStart;
        public event Action<string> OnLoadFinish;

        public void UrlChange(string url)
        {
            unityThread.Post(state =>
            {
                try
                {
                    OnUrlChanged?.Invoke(url);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnUrlChanged! {ex}");
                }
            }, null);
        }

        public void LoadStart(string url)
        {
            unityThread.Post(d =>
            {
                try
                {
                    OnLoadStart?.Invoke(url);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadStart! {ex}");
                }
            }, null);
        }

        public void LoadFinish(string url)
        {
            unityThread.Post(d =>
            {
                try
                {
                    OnLoadFinish?.Invoke(url);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadFinish! {ex}");
                }
            }, null);
        }
    }
}