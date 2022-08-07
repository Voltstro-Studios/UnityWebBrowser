namespace UnityWebBrowser.Shared.Events;

/// <summary>
///     Event related to the keyboard
/// </summary>
public struct KeyboardEvent
{
    /// <summary>
    ///     The keys that were released this frame
    /// </summary>
    public WindowsKey[] KeysUp { get; set; }

    /// <summary>
    ///     The keys that were pressed down this frame
    /// </summary>
    public WindowsKey[] KeysDown { get; set; }

    /// <summary>
    ///     The characters that were pressed this frame
    /// </summary>
    public char[] Chars { get; set; }
}