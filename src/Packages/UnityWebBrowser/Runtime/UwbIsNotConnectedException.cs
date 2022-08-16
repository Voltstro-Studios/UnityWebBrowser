// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser
{
    /// <summary>
    ///     An <see cref="Exception" /> related to when you are trying to do something when the engine is not running
    /// </summary>
    public sealed class UwbIsNotConnectedException : Exception
    {
        internal UwbIsNotConnectedException(string message) : base(message)
        {
        }
    }
}