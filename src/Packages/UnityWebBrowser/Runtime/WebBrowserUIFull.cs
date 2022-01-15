using UnityWebBrowser.Core;

namespace UnityWebBrowser
{
    public class WebBrowserUIFull : RawImageUwbClientInputHandler
    {
        public FullscreenHandler fullscreenHandler = new();

        protected override void OnStart()
        {
            base.OnStart();
            fullscreenHandler.graphicComponent = image;
            browserClient.OnFullscreen += fullscreenHandler.OnEngineFullscreen;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            browserClient.OnFullscreen -= fullscreenHandler.OnEngineFullscreen;
        }
    }
}