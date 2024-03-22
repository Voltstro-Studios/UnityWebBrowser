// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityEngine;
using UnityEngine.UI;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Input;
using VoltstroStudios.UnityWebBrowser.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Prj
{
    /// <summary>
    ///     Demo script for creating UWB at runtime
    /// </summary>
    public sealed class UWBRuntime : MonoBehaviour
    {
        /// <summary>
        ///     <see cref="GameObject"/> to create everything on
        /// </summary>
        [SerializeField]
        private GameObject container;

        [SerializeField]
        private Camera mainCamera;
        
        private void Start()
        {
            //Create canvas
            Canvas canvas = container.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera;

            CanvasScaler canvasScaler = container.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0;

            container.AddComponent<GraphicRaycaster>();
            
            //Child object, where raw image and UWB itself will live
            GameObject uwbGameObject = new("UWBContainer");
            uwbGameObject.transform.SetParent(container.transform);
            
            //Configure rect transform
            RectTransform uwbRectTransform = uwbGameObject.AddComponent<RectTransform>();
            uwbRectTransform.anchorMin = Vector2.zero;
            uwbRectTransform.anchorMax = Vector2.one;
            uwbRectTransform.pivot = new Vector2(0.5f, 0.5f);
            uwbRectTransform.localScale = Vector3.one;
            uwbRectTransform.offsetMin = Vector2.zero;
            uwbRectTransform.offsetMax = Vector2.zero;
            
            //Add raw image
            uwbGameObject.AddComponent<RawImage>();
            
            //UWB Pre-Setup
            
            //Create engine dynamically
            EngineConfiguration engineConfig = ScriptableObject.CreateInstance<EngineConfiguration>();
            engineConfig.engineAppName = "UnityWebBrowser.Engine.Cef";
            
#if UNITY_EDITOR
            engineConfig.engineFiles = new[] { 
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Windows64,
                    engineFileLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.win.x64/Engine~/"
                },
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Linux64,
                    engineFileLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.linux.x64/Engine~/"
                }
            };
#endif
            
            //Create coms layer dynamically
            CommunicationLayer comsLayer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();
            
            //Create input handler dynamically
            WebBrowserInputHandler inputHandler = ScriptableObject.CreateInstance<WebBrowserOldInputHandler>();
            
            //UWB Object Setup
            WebBrowserUIBasic webBrowser = uwbGameObject.AddComponent<WebBrowserUIBasic>();
            webBrowser.browserClient.engine = engineConfig;
            webBrowser.browserClient.communicationLayer = comsLayer;
            webBrowser.inputHandler = inputHandler;
        }
    }
}
