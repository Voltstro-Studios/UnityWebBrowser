using VoltRpc.Proxy;

namespace UnityWebBrowser.Shared
{
    [GenerateProxy(GeneratedName = "ClientProxy")]
    public interface IClient
    {
        public void UrlChange(string url);
    }
}