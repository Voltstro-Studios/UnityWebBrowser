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
}