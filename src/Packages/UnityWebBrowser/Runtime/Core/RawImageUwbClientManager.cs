using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityWebBrowser.Core
{
    [RequireComponent(typeof(RawImage))]
    public abstract class RawImageUwbClientManager : BaseUwbClientManager
    {
        protected RawImage image;
        
        protected override void OnStart()
        {
            image = GetComponent<RawImage>();
            if (image == null)
                throw new NullReferenceException("Game object does not have a raw image component!");

            image.texture = browserClient.BrowserTexture;
            image.uvRect = new Rect(0f, 0f, 1f, -1f);
        }
    }
}