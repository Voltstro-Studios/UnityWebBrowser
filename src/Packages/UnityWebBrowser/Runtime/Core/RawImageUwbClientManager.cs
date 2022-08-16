// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace VoltstroStudios.UnityWebBrowser.Core
{
    /// <summary>
    ///     Implementation of <see cref="BaseUwbClientManager" /> for rendering to a <see cref="RawImage" />
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public abstract class RawImageUwbClientManager : BaseUwbClientManager
    {
        protected RawImage image;

        /// <summary>
        ///     Get the <see cref="RawImage" /> instance
        /// </summary>
        public RawImage Image => image;


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