// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Core.Js
{
    /// <summary>
    ///     An <see cref="Exception"/> related to when arguments have some form of mismatch
    /// </summary>
    public sealed class InvalidArgumentsException : Exception
    {
        internal InvalidArgumentsException(string message) : base(message)
        {
        }
    }
}