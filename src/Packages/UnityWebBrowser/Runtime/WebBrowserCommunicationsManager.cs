using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
            ipcClient.Connect();
        }

        public void Listen()
        {
            ipcHost.StartListening();
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
            lock (threadLock)
            {
                engineProxy.Shutdown();
            }
        }

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            ExecuteTask(() => engineProxy.SendKeyboardEvent(keyboardEvent));
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            ExecuteTask(() => engineProxy.SendMouseMoveEvent(mouseMoveEvent));
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            ExecuteTask(() => engineProxy.SendMouseClickEvent(mouseClickEvent));
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            ExecuteTask(() => engineProxy.SendMouseScrollEvent(mouseScrollEvent));
        }

        public void GoForward()
        {
            ExecuteTask(() => engineProxy.GoForward());
        }

        public void GoBack()
        {
            ExecuteTask(() => engineProxy.GoBack());
        }

        public void Refresh()
        {
            ExecuteTask(() => engineProxy.Refresh());
        }

        public void LoadUrl(string url)
        {
            ExecuteTask(() => engineProxy.LoadUrl(url));
        }

        public void LoadHtml(string html)
        {
            ExecuteTask(() => engineProxy.LoadHtml(html));
        }

        public void ExecuteJs(string js)
        {
            ExecuteTask(() => engineProxy.ExecuteJs(js));
        }

        public void Resize(Resolution resolution)
        {
            ExecuteTask(() => engineProxy.Resize(resolution));
        }

        public void Dispose()
        {
            lock (threadLock)
            {
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
                    logger.Error($"An error occured in OnTitleChange! {ex}");
                }
            }, null);
        }

        public void ProgressChange(double progress)
        {
            unityThread.Post(d =>
            {
                try
                {
                    client.InvokeLoadProgressChange(progress);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadProgressChange! {ex}");
                }
            }, null);
        }

        public void Fullscreen(bool fullScreen)
        {
            unityThread.Post(d =>
            {
                try
                {
                    client.InvokeFullscreen(fullScreen);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnFullscreen! {ex}");
                }
            }, null);
        }

        private void ExecuteTask(Action action)
        {
            _ = Task.Run(() =>
            {
                using (sendEventMarker.Auto())
                    lock (threadLock)
                        action.Invoke();
            });
        }
    }
}