// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using UnityWebBrowser.Engine.Cef.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefRenderHandler" /> implementation
/// </summary>
public class UwbCefRenderHandler : CefRenderHandler
{
    private readonly object pixelsLock;
    private CefSize cefSize;
    private byte[] pixels;

    private readonly ClientControlsActions clientControls;
    
    /// <summary>
    ///     Tracked mouse scroll position
    /// </summary>
    public Vector2 MouseScrollPosition { get; private set; }

    public UwbCefRenderHandler(UwbCefClient client, CefSize size)
    {
        pixelsLock = new object();
        Resize(size);
        clientControls = client.ClientControls;
    }

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

    public void Resize(CefSize size)
    {
        lock (pixelsLock)
        {
            pixels = new byte[size.Width * size.Height * 4];
            cefSize = size;
        }
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

    protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX,
        ref int screenY)
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
    protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects,
        IntPtr buffer, int width,
        int height)
    {
        if (browser != null)
            //Copy our pixel buffer to our pixels
            lock (pixelsLock)
            {
                Marshal.Copy(buffer, pixels, 0, pixels.Length);
            }
    }

    protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
    {
        return false;
    }

    protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
    {
    }

    protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type,
        CefRectangle[] dirtyRects, IntPtr sharedHandle)
    {
    }

    protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
    {
        MouseScrollPosition = new Vector2((float)x, (float)y);
    }

    protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange,
        CefRectangle[] characterBounds)
    {
    }

    protected override void OnVirtualKeyboardRequested(CefBrowser browser, CefTextInputMode inputMode)
    {
        CefLoggerWrapper.Debug($"Input mode changed to: {inputMode}");
        clientControls.InputFocusChange(inputMode == CefTextInputMode.Default);
    }
}