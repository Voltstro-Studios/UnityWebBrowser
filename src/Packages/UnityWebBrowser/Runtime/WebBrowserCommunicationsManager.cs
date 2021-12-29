using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    /// <summary>
    ///     Handles the RPC methods and two-way communication between the UWB engine and Unity
    /// </summary>
    internal class WebBrowserCommunicationsManager : IEngine, IClient, IDisposable
    {
        private static ProfilerMarker sendEventMarker = new ("UWB.SendEvent");
        
        private readonly IEngine engineProxy;
        private readonly Client ipcClient;
        private readonly Host ipcHost;

        private readonly SynchronizationContext unityThread;
        private readonly object threadLock;
        
        private readonly IWebBrowserLogger logger;
        private readonly WebBrowserClient client;

        /// <summary>
        ///     Is our client connected to the UWB engine
        /// </summary>
        public bool IsConnected => ipcClient.IsConnected;

        /// <summary>
        ///     Creates a new <see cref="WebBrowserCommunicationsManager"/> instance
        /// </summary>
        /// <param name="browserClient"></param>
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

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent) => 
            ExecuteTask(() => engineProxy.SendKeyboardEvent(keyboardEvent));

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent) => 
            ExecuteTask(() => engineProxy.SendMouseMoveEvent(mouseMoveEvent));

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent) => 
            ExecuteTask(() => engineProxy.SendMouseClickEvent(mouseClickEvent));

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent) => 
            ExecuteTask(() => engineProxy.SendMouseScrollEvent(mouseScrollEvent));

        public void GoForward() => ExecuteTask(() => engineProxy.GoForward());

        public void GoBack() => ExecuteTask(() => engineProxy.GoBack());

        public void Refresh() => ExecuteTask(() => engineProxy.Refresh());

        public void LoadUrl(string url) => ExecuteTask(() => engineProxy.LoadUrl(url));

        public void LoadHtml(string html) => ExecuteTask(() => engineProxy.LoadHtml(html));

        public void ExecuteJs(string js) => ExecuteTask(() => engineProxy.ExecuteJs(js));

        public void Resize(Resolution resolution) => ExecuteTask(() => engineProxy.Resize(resolution));

        #region Client Events

        public void UrlChange(string url) => ExecuteOnUnity(() => client.InvokeUrlChanged(url));

        public void LoadStart(string url) => ExecuteOnUnity(() => client.InvokeLoadStart(url));

        public void LoadFinish(string url) => ExecuteOnUnity(() => client.InvokeLoadFinish(url));

        public void TitleChange(string title) => ExecuteOnUnity(() => client.InvokeTitleChange(title));

        public void ProgressChange(double progress) =>
            ExecuteOnUnity(() => client.InvokeLoadProgressChange(progress));

        public void Fullscreen(bool fullScreen) => ExecuteOnUnity(() => client.InvokeFullscreen(fullScreen));

        #endregion
        
        public void Dispose()
        {
            lock (threadLock)
            {
                ipcHost?.Dispose();
                ipcClient?.Dispose();
            }
        }

        private void ExecuteOnUnity(Action action, [CallerMemberName] string memberName = "")
        {
            unityThread.Post(_ =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured in {memberName}! {ex}");
                }
            }, null);
        }

        private void ExecuteTask(Action action, [CallerMemberName] string memberName = "")
        {
            if(!IsConnected)
                return;

            UniTask.Run(() =>
            {
                sendEventMarker.Begin();
                try
                {
                    lock (threadLock)
                        action.Invoke();
                }
                catch (Exception ex)
                {
                    logger.Error($"An error occured while executing task {memberName}! {ex}");
                }
                sendEventMarker.End();
            });
        }
    }
}