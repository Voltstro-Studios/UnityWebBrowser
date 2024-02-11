// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Core.Js
{
    public sealed class NotEnabledException : Exception
    {
        public NotEnabledException(string message) : base(message)
        {
        }
    }
}