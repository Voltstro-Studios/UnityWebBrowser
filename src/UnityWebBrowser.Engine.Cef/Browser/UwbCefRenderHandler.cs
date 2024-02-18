// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityWebBrowser.Engine.Cef.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefRenderHandler" /> implementation
/// </summary>
internal class UwbCefRenderHandler : CefRenderHandler
{
    private CefSize cefSize;

    private readonly object pixelsLock;
    private Memory<byte> userPixelsBuffer;
    private byte[] pixelsBuffer;
    private int pixelsLength;

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
    
    public ReadOnlyMemory<byte> Pixels
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            lock (pixelsLock)
            {
                pixelsBuffer.CopyTo(userPixelsBuffer);
            }

            return userPixelsBuffer;
        }
    }

    public void Resize(CefSize size)
    {
        pixelsLength = size.Width * size.Height * 4;
        
        lock (pixelsLock)
        {
            pixelsBuffer = new byte[pixelsLength];
            userPixelsBuffer = new Memory<byte>(new byte[pixelsLength]);
        }
        
        cefSize = size;
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
        //Ensure buffer sizes are the same
        int myBufferSize = width * height * 4;
        if(myBufferSize != pixelsLength)
            return;
        
        //Copy our pixel buffer to our pixels
        lock (pixelsLock)
        {
            Marshal.Copy(buffer, pixelsBuffer, 0, pixelsLength);
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