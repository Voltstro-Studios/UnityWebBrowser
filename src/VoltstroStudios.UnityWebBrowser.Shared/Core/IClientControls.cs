// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.Proxy;

namespace VoltstroStudios.UnityWebBrowser.Shared.Core;

/// <summary>
///     Shared interface for events that client has
/// </summary>
[GenerateProxy(GeneratedName = "ClientControls", GeneratedNamespace = "VoltstroStudios.UnityWebBrowser.Shared.Core")]
internal interface IClientControls
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
    ///     The input's focus has changed
    /// </summary>
    /// <param name="focused"></param>
    public void InputFocusChange(bool focused);

    /// <summary>
    ///     Tell the client that we are ready
    /// </summary>
    public void Ready();
}