using System;
using System.Runtime.InteropServices;
using System.Security;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
	public class BrowserProcessCEFRenderHandler : CefRenderHandler
	{
		private readonly CefSize cefSize;

        private readonly object pixelsLock;
        private byte[] pixels;
        public byte[] Pixels
        {
	        get
	        {
		        lock (pixelsLock)
		        {
			        byte[] pixelsCopyBuffer = new byte[pixels.Length];
			        Array.Copy(pixels, pixelsCopyBuffer, pixels.Length);
			        return pixelsCopyBuffer;
		        }
	        }
        }

        public BrowserProcessCEFRenderHandler(CefSize size)
        {
	        cefSize = size;
	        pixels = new byte[size.Width * size.Height * 4];
	        pixelsLock = new object();
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
        	return null;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
        	GetViewRect(browser, out CefRectangle newRect);
        	rect = newRect;
        	return true;
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
        	screenX = viewX;
        	screenY = viewY;
        	return true;
        }

        protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
        {
        	rect = new CefRectangle(0, 0, cefSize.Width, cefSize.Height);
        }

        [SecurityCritical]
        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width,
        	int height)
        {
        	if (browser != null)
        	{
	            lock(pixelsLock)
	            {
		            Marshal.Copy(buffer, pixels, 0, pixels.Length);
	            }
            }
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
        	return false;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
        }
    }
}