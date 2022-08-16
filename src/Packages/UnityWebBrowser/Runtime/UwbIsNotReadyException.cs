// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser
{
    /// <summary>
    ///     An <see cref="Exception" /> when UWB is not ready
    /// </summary>
    public sealed class UwbIsNotReadyException : Exception
    {
        internal UwbIsNotReadyException(string message)
            : base(message)
        {
        }
    }
}