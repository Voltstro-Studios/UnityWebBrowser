using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityWebBrowser.Core
{
    /// <summary>
    ///     Handles dealing with fullscreen stuff
    /// </summary>
    [Serializable]
    public sealed class FullscreenHandler
    {
        [NonSerialized] public Graphic graphicComponent;
        
        /// <summary>
        ///     What objects to hide when the browser wants to be in fullscreen mode
        /// </summary>
        [Tooltip("What objects to hide when the browser wants to be in fullscreen mode")]
        public GameObject[] hideOnFullscreen;

        private Vector2 lastGraphicSize;
        private Vector2 lastGraphicMin;
        private Vector2 lastGraphicMax;
        private Vector2 lastGraphicPosition;

        public void OnEngineFullscreen(bool fullscreen)
        {
            RectTransform graphicRectTransform = graphicComponent.rectTransform;
            
            if (fullscreen)
            {
                foreach (GameObject obj in hideOnFullscreen)
                    obj.SetActive(false);

                
                lastGraphicSize = graphicRectTransform.sizeDelta;
                lastGraphicMax = graphicRectTransform.anchorMax;
                lastGraphicMin = graphicRectTransform.anchorMin;
                lastGraphicPosition = graphicRectTransform.anchoredPosition;

                graphicRectTransform.anchoredPosition = Vector2.zero;
                graphicRectTransform.anchorMin = Vector2.zero;
                graphicRectTransform.anchorMax = Vector2.one;
                graphicRectTransform.sizeDelta = Vector2.zero;
            }
            else
            {
                foreach (GameObject obj in hideOnFullscreen)
                    obj.SetActive(true);
                
                graphicRectTransform.anchoredPosition = lastGraphicPosition;
                graphicRectTransform.anchorMin = lastGraphicMin;
                graphicRectTransform.anchorMax = lastGraphicMax;
                graphicRectTransform.sizeDelta = lastGraphicSize;
            }
        }
    }
}