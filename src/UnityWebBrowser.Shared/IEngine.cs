using System.Net;
using ServiceWire;
using ServiceWire.TcpIp;
using UnityWebBrowser.Shared.Events.EngineAction;

namespace UnityWebBrowser.Shared
{
    public interface IEngine
    {
        public byte[] GetPixels();

        public void Shutdown();

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent);
        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent);
        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent);
        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent);

        public void GoForward();
        public void GoBack();
        public void Refresh();
        public void LoadUrl(string url);
        public void LoadHtml(string html);
        public void ExecuteJs(string js);
    }

    public class EngineProxy : TcpChannel, IEngine
    {
        public EngineProxy(IPEndPoint endPoint, ISerializer serializer)
            : base(typeof(IEngine), endPoint, serializer)
        {
        }

        public byte[] GetPixels()
        {
            return (byte[]) InvokeMethod(nameof(GetPixels))[0];
        }

        public void Shutdown()
        {
            InvokeMethod(nameof(Shutdown));
        }

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            InvokeMethod(
                $"{nameof(SendKeyboardEvent)}|UnityWebBrowser.Shared.Events.EngineAction.{nameof(KeyboardEvent)}, UnityWebBrowser.Shared", keyboardEvent);
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            InvokeMethod(
                $"{nameof(SendMouseMoveEvent)}|UnityWebBrowser.Shared.Events.EngineAction.{nameof(MouseMoveEvent)}, UnityWebBrowser.Shared", mouseMoveEvent);
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            InvokeMethod(
                $"{nameof(SendMouseClickEvent)}|UnityWebBrowser.Shared.Events.EngineAction.{nameof(MouseClickEvent)}, UnityWebBrowser.Shared", mouseClickEvent);
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            InvokeMethod(
                $"{nameof(SendMouseScrollEvent)}|UnityWebBrowser.Shared.Events.EngineAction.{nameof(MouseScrollEvent)}, UnityWebBrowser.Shared", mouseScrollEvent);
        }

        public void GoForward()
        {
            InvokeMethod(nameof(GoForward));
        }

        public void GoBack()
        {
            InvokeMethod(nameof(GoBack));
        }

        public void Refresh()
        {
            InvokeMethod(nameof(Refresh));
        }

        public void LoadUrl(string url)
        {
            InvokeMethod(nameof(LoadUrl), url);
        }

        public void LoadHtml(string html)
        {
            InvokeMethod(nameof(LoadHtml), html);
        }

        public void ExecuteJs(string js)
        {
            InvokeMethod(nameof(ExecuteJs), js);
        }
    }
}