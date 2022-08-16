// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

public class CefActionTask : CefTask
{
    private Action action;

    public CefActionTask(Action action)
    {
        this.action = action;
    }

    protected override void Execute()
    {
        action();
        action = null;
    }
}