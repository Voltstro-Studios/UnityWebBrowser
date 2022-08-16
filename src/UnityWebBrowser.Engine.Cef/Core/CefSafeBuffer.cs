// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Runtime.InteropServices;

//From: https://github.com/chromelyapps/Chromely/blob/989d74141aabb8d874b2ad9b75757f56f3e6fdba/src_5.2/Chromely/Browser/Handlers/CefSafeBuffer.cs
namespace UnityWebBrowser.Engine.Cef.Core;

public class CefSafeBuffer : SafeBuffer
{
    public CefSafeBuffer(IntPtr data, ulong noOfBytes) : base(false)
    {
        SetHandle(data);
        Initialize(noOfBytes);
    }

    protected override bool ReleaseHandle()
    {
        return true;
    }
}