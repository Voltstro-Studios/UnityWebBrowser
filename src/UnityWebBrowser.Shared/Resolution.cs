using System;

namespace UnityWebBrowser.Shared
{
    /// <summary>
    ///     Screen resolution
    /// </summary>
    [Serializable]
    public struct Resolution
    {
        /// <summary>
        ///     Creates a new <see cref="Resolution"/> instance
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Resolution(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Width of the screen
        /// </summary>
        public uint Width;

        /// <summary>
        ///     Height of the screen
        /// </summary>
        public uint Height;
    }
}