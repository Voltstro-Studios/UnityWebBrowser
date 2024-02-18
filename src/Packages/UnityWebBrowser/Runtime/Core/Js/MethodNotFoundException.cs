// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Core.Js
{
    /// <summary>
    ///     An <see cref="Exception"/> to when a method name cannot be found
    /// </summary>
    public sealed class MethodNotFoundException : Exception
    {
        internal MethodNotFoundException(string message) : base(message)
        {
        }
    }
}