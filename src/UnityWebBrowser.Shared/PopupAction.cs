namespace UnityWebBrowser.Shared;

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