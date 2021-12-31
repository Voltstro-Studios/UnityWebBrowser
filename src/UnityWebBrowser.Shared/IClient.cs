using VoltRpc.Proxy;

namespace UnityWebBrowser.Shared
{
    [GenerateProxy(GeneratedName = "ClientProxy")]
    public interface IClient
    {
        public void UrlChange(string url);

        public void LoadStart(string url);

        public void LoadFinish(string url);

        public void TitleChange(string title);

        public void ProgressChange(double progress);

        public void Fullscreen(bool fullScreen);

        public void Ready();
    }
}