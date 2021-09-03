using UnityWebBrowser.Shared.Events;
using VoltRpc.Proxy;

namespace UnityWebBrowser.Shared
{
    [GenerateProxy(GeneratedName = "EngineProxy")]
    public interface IEngine
    {
        public PixelsEvent GetPixels();

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

        public void Resize(Resolution resolution);
    }
}