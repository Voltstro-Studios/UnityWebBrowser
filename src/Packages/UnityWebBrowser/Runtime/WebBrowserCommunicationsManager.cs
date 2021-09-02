using System;
using System.Net;
using System.Threading;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Delegates;
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
        private readonly object threadLock;
        private readonly IWebBrowserLogger logger;

        public bool IsConnected => client.IsConnected;

        public WebBrowserCommunicationsManager(WebBrowserIpcSettings ipcSettings, IWebBrowserLogger logger)
        {
            threadLock = new object();
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
            lock (threadLock)
            {
                client.Connect();
            }
        }

        public void Listen()
        {
            lock (threadLock)
            {
                host.StartListening();
            }
        }

        public PixelsEvent GetPixels()
        {
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
            lock (threadLock)
            {
                engineProxy.SendKeyboardEvent(keyboardEvent);
            }
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            lock (threadLock)
            {
                engineProxy.SendMouseMoveEvent(mouseMoveEvent);
            }
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            lock (threadLock)
            {
                engineProxy.SendMouseClickEvent(mouseClickEvent);
            }
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            lock (threadLock)
            {
                engineProxy.SendMouseScrollEvent(mouseScrollEvent);
            }
        }

        public void GoForward()
        {
            lock (threadLock)
            {
                engineProxy.GoForward();
            }
        }

        public void GoBack()
        {
            lock (threadLock)
            {
                engineProxy.GoBack();
            }
        }

        public void Refresh()
        {
            lock (threadLock)
            {
                engineProxy.Refresh();
            }
        }

        public void LoadUrl(string url)
        {
            lock (threadLock)
            {
                engineProxy.LoadUrl(url);
            }
        }

        public void LoadHtml(string html)
        {
            lock (threadLock)
            {
                engineProxy.LoadHtml(html);
            }
        }

        public void ExecuteJs(string js)
        {
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
                client.Dispose();
                host.Dispose();
            }
        }

        public OnUrlChangeDelegate OnUrlChanged;
        public OnLoadStartDelegate OnLoadStart;
        public OnLoadFinishDelegate OnLoadFinish;
        public OnTitleChange OnTitleChange;

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

        public void TitleChange(string title)
        {
            unityThread.Post(d =>
            {
                try
                {
                    OnTitleChange?.Invoke(title);
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in OnLoadFinish! {ex}");
                }
            }, null);
        }
    }
}