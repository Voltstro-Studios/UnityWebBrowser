using VoltRpc.Proxy;

namespace UnityWebBrowser.Shared.Core;

/// <summary>
///     Shared interface for events that client has
/// </summary>
[GenerateProxy(GeneratedName = "ClientProxy", GeneratedNamespace = "UnityWebBrowser.Shared.Proxy")]
public interface IClient
{
    /// <summary>
    ///     Tell the client that the URL changed
    /// </summary>
    /// <param name="url"></param>
    public void UrlChange(string url);

    /// <summary>
    ///     Tell the client that a URL started to load
    /// </summary>
    /// <param name="url"></param>
    public void LoadStart(string url);

    /// <summary>
    ///     Tells the client that a popup has been invoked
    /// </summary>
    /// <param name="url"></param>
    public void OnPopup(string url);

    /// <summary>
    ///     Tell the client that a URL finished
    /// </summary>
    /// <param name="url"></param>
    public void LoadFinish(string url);

    /// <summary>
    ///     Tell the client that the title changed
    /// </summary>
    /// <param name="title"></param>
    public void TitleChange(string title);

    /// <summary>
    ///     Tell the client about the loading progress changed
    /// </summary>
    /// <param name="progress"></param>
    public void ProgressChange(double progress);

    /// <summary>
    ///     Tell the client about a change in fullscreen
    /// </summary>
    /// <param name="fullScreen"></param>
    public void Fullscreen(bool fullScreen);

    /// <summary>
    ///     Tell the client that we are ready
    /// </summary>
    public void Ready();
}