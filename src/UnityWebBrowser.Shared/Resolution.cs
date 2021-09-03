namespace UnityWebBrowser.Shared
{
    public struct Resolution
    {
        public Resolution(uint width, uint height)
        {
            Width = width;
            Height = height;
        }
        
        public Resolution(int width, int height)
        {
            Width = (uint)width;
            Height = (uint)height;
        }
        
        public uint Width { get; }
        
        public uint Height { get; }
    }
}