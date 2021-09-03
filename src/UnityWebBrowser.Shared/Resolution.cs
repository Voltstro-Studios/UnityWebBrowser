using System;

namespace UnityWebBrowser.Shared
{
    [Serializable]
    public struct Resolution
    {
        public Resolution(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        public uint Width;

        public uint Height;
    }
}