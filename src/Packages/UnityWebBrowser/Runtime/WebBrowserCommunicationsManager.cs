using System;
using System.Net;
using System.Threading;
using Unity.Profiling;
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
        private static ProfilerMarker sendEventMarker = new ProfilerMarker("UWB.SendEvent");
        
        private readonly IEngine engineProxy;
        private readonly Client ipcClient;
        private readonly Host ipcHost;

        private readonly SynchronizationContext unityThread;
        private readonly object threadLock;
        
        private readonly IWebBrowserLogger logger;
        private readonly WebBrowserClient client;

        public bool IsConnected => ipcClient.IsConnected;

        public WebBrowserCommunicationsManager(WebBrowserClient browserClient)
        {
            threadLock = new object();
            unityThread = SynchronizationContext.Current;
            
            logger = browserClient.logger;
            client = browserClient;

            if (browserClient.ipcSettings.preferPipes)
            {
                logger.Debug("Using pipes communication...");

                ipcHost = new PipesHost(browserClient.ipcSettings.inPipeName);
                ipcClient = new PipesClient(browserClient.ipcSettings.outPipeName, browserClient.ipcSettings.connectionTimeout,
                    Client.DefaultBufferSize);
            }
            else
            {
                logger.Debug("Using TCP communication...");

                ipcHost = new TCPHost(new IPEndPoint(IPAddress.Loopback, (int)browserClient.ipcSettings.inPort));
                ipcClient = new TCPClient(new IPEndPoint(IPAddress.Loopback, (int)browserClient.ipcSettings.outPort));
            }

            ReadWriterUtils.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
            ipcHost.AddService<IClient>(this);

            ReadWriterUtils.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
            ipcClient.AddService<IEngine>();
            engineProxy = new EngineProxy(ipcClient);
        }

        public void Connect()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    ipcClient.Connect();
                }
        }

        public void Listen()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    ipcHost.StartListening();
                }
        }

        public PixelsEvent GetPixels()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    return engineProxy.GetPixels();
                }
        }

        public void Shutdown()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.Shutdown();
                }
        }

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.SendKeyboardEvent(keyboardEvent);
                }
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.SendMouseMoveEvent(mouseMoveEvent);
                }
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.SendMouseClickEvent(mouseClickEvent);
                }
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.SendMouseScrollEvent(mouseScrollEvent);
                }
        }

        public void GoForward()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.GoForward();
                }
        }

        public void GoBack()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.GoBack();
                }
        }

        public void Refresh()
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.Refresh();
                }
        }

        public void LoadUrl(string url)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.LoadUrl(url);
                }
        }

        public void LoadHtml(string html)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.LoadHtml(html);
                }
        }

        public void ExecuteJs(string js)
        {
            using (sendEventMarker.Auto())
                lock (threadLock)
                {
                    engineProxy.ExecuteJs(js);
                }
        }

        public void Dispose()
        {
            lock (threadLock)
            {
                Shutdown();
                ipcClient.Dispose();
                ipcHost.Dispose();
            }
        }

        public void UrlChange(string url)
        {
            unityThread.Post(state =>
            {
                try
                {
                    client.InvokeUrlChanged(url);
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
                    client.InvokeLoadStart(url);
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
                    client.InvokeLoadFinish(url);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadFinish! {ex}");
                }
            }, null);
        }

        public void TitleChange(string title)
        {
            unityThread.Post(d =>
            {
                try
                {
                    client.InvokeTitleChange(title);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadFinish! {ex}");
                }
            }, null);
        }
    }
}