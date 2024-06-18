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
    
    private int viewWidth;
    private int viewHeight;
    
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

    #region Popups

    //Popups are widgets, like the select element

    private int popupDataBufferSize;
    private byte[] popupDataBuffer;
    
    private bool showPopup;

    private int popupX;
    private int popupY;
    
    private int popupWidth;
    private int popupHeight;
    
    protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
    {
        int popupDataLength = rect.Width * rect.Height * 4;
        if (popupDataBufferSize == popupDataLength || !showPopup)
            return;
        
        popupDataBuffer = new byte[popupDataLength];
        popupDataBufferSize = popupDataLength;

        popupWidth = rect.Width * 4;
        popupHeight = rect.Height * 4;
        popupX = rect.X;
            
        popupY = rect.Y;
    }

    protected override void OnPopupShow(CefBrowser browser, bool show)
    {
        showPopup = show;
    }

    #endregion

    [SecurityCritical]
    protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
    {
        if (type == CefPaintElementType.Popup && showPopup)
        {
            Marshal.Copy(buffer, popupDataBuffer, 0, popupDataBufferSize);
            DrawPopupToMainBuffer();
            return;
        }
        
        //Ensure buffer sizes are the same
        int myBufferSize = width * height * 4;
        if(myBufferSize != pixelsLength)
            return;

        viewHeight = height;
        viewWidth = width;
        
        //Copy our pixel buffer to our pixels
        lock (pixelsLock)
        {
            Marshal.Copy(buffer, pixelsBuffer, 0, pixelsLength);

            //Redraw popup if one is ment to be showing
            if (showPopup)
                DrawPopupToMainBuffer();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawPopupToMainBuffer()
    {
        int popupDataIndex = 0;
        for (int y = 0; y < popupHeight * viewWidth; y += 4 * viewWidth)
        {
            for (int x = 0; x < popupWidth; x++)
            {
                pixelsBuffer[y + x] = popupDataBuffer[popupDataIndex];
                popupDataIndex++;
            }
        }
    }

    protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
    {
        return false;
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