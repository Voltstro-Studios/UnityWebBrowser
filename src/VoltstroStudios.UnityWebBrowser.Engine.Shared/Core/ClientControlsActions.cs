// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using VoltRpc.Communication;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

#nullable enable
namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     This is a wrapper around <see cref="IClientControls" />. It checks if we are connected first before firing an event.
///     <para>The reason why we do the check here is that VoltRpc will throw an exception, rather then not do anything.</para>
/// </summary>
public class ClientControlsActions : IClientControls, IDisposable
{
    private Client? client;
    private IClientControls? clientActions;

    public void UrlChange(string url)
    {
        if (client is {IsConnected: true})
            clientActions?.UrlChange(url);
    }

    public void LoadStart(string url)
    {
        if (client is {IsConnected: true})
            clientActions?.LoadStart(url);
    }

    public void LoadFinish(string url)
    {
        if (client is {IsConnected: true})
            clientActions?.LoadFinish(url);
    }

    public void TitleChange(string title)
    {
        if (client is {IsConnected: true})
            clientActions?.TitleChange(title);
    }

    public void ProgressChange(double progress)
    {
        if (client is {IsConnected: true})
            clientActions?.ProgressChange(progress);
    }

    public void Fullscreen(bool fullScreen)
    {
        if (client is {IsConnected: true})
            clientActions?.Fullscreen(fullScreen);
    }

    public void InputFocusChange(bool focused)
    {
        if (client is {IsConnected: true})
            clientActions?.InputFocusChange(focused);
    }

    public void Ready()
    {
        if (client is {IsConnected: true})
            clientActions?.Ready();
    }

    public void ExecuteJsMethod(ExecuteJsMethod executeJsMethod)
    {
        if (client is {IsConnected: true})
            clientActions?.ExecuteJsMethod(executeJsMethod);
    }

    public void Dispose()
    {
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    internal void SetIpcClient(Client ipcClient)
    {
        client = ipcClient ?? throw new NullReferenceException();
        clientActions = new ClientControls(client);
    }

    ~ClientControlsActions()
    {
        ReleaseResources();
    }

    private void ReleaseResources()
    {
        client?.Dispose();
    }
}