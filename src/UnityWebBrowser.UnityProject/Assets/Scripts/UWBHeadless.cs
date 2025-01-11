using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Prj
{
    public class UWBHeadless : MonoBehaviour
    {
        private WebBrowserClient webBrowserClient;
        
        // Start is called before the first frame update
        public void Start()
        {
            //Create web browser client with headless set to true
            webBrowserClient = new WebBrowserClient(true);
            
            //Create engine dynamically
            EngineConfiguration engineConfig = ScriptableObject.CreateInstance<EngineConfiguration>();
            engineConfig.engineAppName = "UnityWebBrowser.Engine.Cef";
            
#if UNITY_EDITOR
            engineConfig.engineFiles = new[] { 
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Windows64,
                    engineBaseAppLocation = string.Empty,
                    engineEditorLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.win.x64/Engine~/",
                    engineRuntimeLocation = "UWB/"
                },
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Linux64,
                    engineBaseAppLocation = string.Empty,
                    engineEditorLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.linux.x64/Engine~/",
                    engineRuntimeLocation = "UWB/"
                },
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.MacOS,
                    engineBaseAppLocation = "UnityWebBrowser.Engine.Cef.app/Contents/MacOS/",
                    engineEditorLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.macos.x64/Engine~/",
                    engineRuntimeLocation = "Frameworks/"
                },
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.MacOSArm64,
                    engineBaseAppLocation = "UnityWebBrowser.Engine.Cef.app/Contents/MacOS/",
                    engineEditorLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.macos.arm64/Engine~/",
                    engineRuntimeLocation = "Frameworks/"
                }
            };
#endif
            
            //Create coms layer dynamically
            CommunicationLayer comsLayer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();
            
            webBrowserClient.engine = engineConfig;
            webBrowserClient.communicationLayer = comsLayer;
            
            webBrowserClient.Init();
        }

        public void OnDestroy()
        {
            webBrowserClient.Dispose();
        }

        public void OpenDevTools()
        {
            webBrowserClient.OpenDevTools();
        }

        public void ExecuteJs()
        {
            webBrowserClient.ExecuteJs("console.log('Hello World!');");
        }
    }
}
