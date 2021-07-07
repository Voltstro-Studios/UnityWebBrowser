using UnityWebBrowser.Shared.Events.EngineAction;

namespace UnityWebBrowser.Shared
{
    public interface IEngine
    {
        public byte[] GetPixels();

        public void Shutdown();
        
        
    }
}