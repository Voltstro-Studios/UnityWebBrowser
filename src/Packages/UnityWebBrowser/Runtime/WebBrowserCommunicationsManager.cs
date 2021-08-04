using System;
using System.Net;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using UnityWebBrowser.Shared.Events.ReadWriters;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;
using VoltRpc.Proxy.Generated;

namespace UnityWebBrowser
{
    internal class WebBrowserCommunicationsManager : IEngine, IDisposable
    {
        private readonly IEngine engineProxy;
        private readonly Client client;
        
        public WebBrowserCommunicationsManager(int connectionTimeout, int outPort)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, outPort);
            client = new TCPClient(ip, connectionTimeout);
            ReadWriterUtils.AddTypeReadWriters(client.TypeReaderWriterManager);
            client.AddService<IEngine>();
            engineProxy = new EngineProxy(client);
        }

        public void Connect()
        {
            client.Connect();
        }

        public byte[] GetPixels() => engineProxy.GetPixels();

        public void Shutdown() => engineProxy.Shutdown();

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent) => engineProxy.SendKeyboardEvent(keyboardEvent);

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent) => engineProxy.SendMouseMoveEvent(mouseMoveEvent);

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent) => engineProxy.SendMouseClickEvent(mouseClickEvent);

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent) =>
            engineProxy.SendMouseScrollEvent(mouseScrollEvent);

        public void GoForward() => engineProxy.GoForward();

        public void GoBack() => engineProxy.GoBack();

        public void Refresh() => engineProxy.Refresh();

        public void LoadUrl(string url) => engineProxy.LoadUrl(url);

        public void LoadHtml(string html) => engineProxy.LoadHtml(html);

        public void ExecuteJs(string js) => engineProxy.ExecuteJs(js);

        public void Dispose()
        {
            Shutdown();
            client.Dispose();
        }
    }
}