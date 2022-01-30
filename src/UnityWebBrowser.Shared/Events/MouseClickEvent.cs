namespace UnityWebBrowser.Shared.Events;

/// <summary>
///     What the mouse click type is
/// </summary>
public enum MouseClickType : byte
{
    /// <summary>
    ///     Left button
    /// </summary>
    Left,
    
    /// <summary>
    ///     Middle button (scroll-wheel)
    /// </summary>
    Middle,
    
    /// <summary>
    ///     Right button
    /// </summary>
    Right
}

/// <summary>
///     What event type was the mouse event
/// </summary>
public enum MouseEventType : byte
{
    /// <summary>
    ///     The button was pressed down
    /// </summary>
    Down,
    
    /// <summary>
    ///     The button was pressed up
    /// </summary>
    Up
}

/// <summary>
///     Event for a mouse click
/// </summary>
public struct MouseClickEvent
{
    /// <summary>
    ///     The X position the mouse was clicked at
    /// </summary>
    public int MouseX { get; set; }
    
    /// <summary>
    ///     The Y position the mouse was clicked at
    /// </summary>
    public int MouseY { get; set; }
    
    /// <summary>
    ///     How many times was the mouse clicked
    /// </summary>
    public int MouseClickCount { get; set; }
    
    /// <summary>
    ///     What was the <see cref="MouseClickType"/>
    /// </summary>
    public MouseClickType MouseClickType { get; set; }
    
    /// <summary>
    ///     What was the <see cref="MouseEventType"/>
    /// </summary>
    public MouseEventType MouseEventType { get; set; }
}