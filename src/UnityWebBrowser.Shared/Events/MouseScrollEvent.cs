namespace UnityWebBrowser.Shared.Events;

/// <summary>
///     Event related to when the mouse scrolls
/// </summary>
public struct MouseScrollEvent
{
    /// <summary>
    ///     The X position the moused moved to
    /// </summary>
    public int MouseX { get; set; }
    
    /// <summary>
    ///     The Y position the moused moved to
    /// </summary>
    public int MouseY { get; set; }
    
    /// <summary>
    ///     How much did ths mouse move
    /// </summary>
    public int MouseScroll { get; set; }
}