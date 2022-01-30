namespace UnityWebBrowser.Shared.Events;

/// <summary>
///     Event related to when the mouse moves
/// </summary>
public struct MouseMoveEvent
{
    /// <summary>
    ///     The X position the moused moved to
    /// </summary>
    public int MouseX { get; set; }
    
    /// <summary>
    ///     The Y position the mouse moved to
    /// </summary>
    public int MouseY { get; set; }
}