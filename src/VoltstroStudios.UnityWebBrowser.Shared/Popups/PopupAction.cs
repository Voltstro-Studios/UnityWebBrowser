// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Popups;

/// <summary>
///     What to do on a popup
/// </summary>
public enum PopupAction : byte
{
    /// <summary>
    ///     Ignore it
    /// </summary>
    Ignore,
    
    /// <summary>
    ///     Open an external window for it
    /// </summary>
    OpenExternalWindow,
    
    /// <summary>
    ///     Redirect the current window to it
    /// </summary>
    Redirect
}