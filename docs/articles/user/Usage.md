# Usage

Alright, lets get to actually using UWB.

## Platform Support

UWB aims to support all desktop platforms (Windows, Linux, MacOS). The core currently does support all desktop platforms, however you will have to factor in what engine you want to use. Each engine has their own different platform support. See the [engine page](Engines.md) for each engine's platform support.

> [!WARNING]
> UWB does **NOT** support [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html)!
>
> UWB does however support being code trimmed.

## Components

By default, UWB provides two different components for handling web viewing from a <xref:UnityEngine.UI.RawImage>:

- <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIBasic>
- <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull>

While both of these components are fundamentally the same (they both inherit from <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager>), the <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull> has an additional <xref:VoltstroStudios.UnityWebBrowser.Core.FullscreenHandler> for users who want fullscreen controls.

A <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIControls> components is also provided, to wrap around some methods provided in <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> and expose them to Unity's UI system.

## Options

Most options that you will ever need is in the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient>, which is the core class for interfacing with UWB.

A lot options are exposed in the editor.

![Web Browser Basic](~/assets/images/articles/user/usage/UWBOptions.webp)

The options are all very self-explanatory. If you need more info about one, hover over it for it's tooltip. Some options are explained in more details further along in the docs.

For calling methods on the engine (such as going back/forward/refresh, loading HTML, executing JS, etc...) are provided by the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> (see the API docs for a full reference). Any component that inherits from <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager> will have the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> exposed via <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager.browserClient> field.

Some example code of loading a website from a method would look like:

```csharp
using UnityEngine;
using VoltstroStudios.UnityWebBrowser.Core;

public class ExampleLoadingSite : MonoBehaviour
{
    //SerializeField exposes the control in Unity
    [SerializeField] private BaseUwbClientManager clientManager;
        
    private WebBrowserClient webBrowserClient;

    private void Start()
    {
        //Makes life easier having a local reference to WebBrowserClient
        webBrowserClient = clientManager.browserClient;
    }

    //Call this from were ever, and it will load 'https://voltstro.dev'
    public void LoadMySite()
    {
        webBrowserClient.LoadUrl("https://voltstro.dev");
    }
}
```

## Samples

You can import one of the samples UWB includes via the package manager. Open the Package Manager and go to the UWB package, you will see the samples and be able to import them into your project.

![UPM Sample](~/assets/images/articles/user/usage/UPMSamples.webp)
