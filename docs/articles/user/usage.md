# Usage

Alright, lets get to actually using UWB.

## Components

By default, UWB provides two different components for handling web viewing from a [Raw Image](https://docs.unity3d.com/2021.3/Documentation/Manual/script-RawImage.html):

- <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIBasic>
- <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull>

While both of these components are fundamentally the same (they both inherit from <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager>), the <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull> has an additional <xref:VoltstroStudios.UnityWebBrowser.Core.FullscreenHandler> for users who want fullscreen controls.

A <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIControls> component is also provided, to wrap around some methods provided in <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> and expose them to Unity's UI system.

## Basic Setup

At a bare minimum, you will need a gameobject with a raw image component and either a <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIBasic> or <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull> component attached. Both components need to live on the same game object.

Once added, configure options with an engine, a communication layer and an input handler, and you should be ready to go.

![Basic Setup](~/assets/images/articles/user/usage/BasicSetup.webp)

If you are still having difficulties setting up the components, [take a look at the sample](#samples).

## Options

Most options that you will ever need is in the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient>, which is the core class for interfacing with UWB.

A lot of options are exposed in the editor.

![Options](~/assets/images/articles/user/usage/UWBOptions.webp)

The options are all very self-explanatory. If you need more info about one, hover over it for it's tooltip. Some options are explained in more details further along in the docs.

## API

For calling methods on the engine (such as going back/forward/refresh, loading HTML, executing JS, etc...) are provided by the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> (see the API docs for a full reference). The <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> lives as a property on <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager>. Any component that inherits from <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager> (such as <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull>) will have the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> exposed via <xref:VoltstroStudios.UnityWebBrowser.Core.BaseUwbClientManager.browserClient> property.

Some example code of loading a website from a method would look like:

```csharp
using UnityEngine;
using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Core;

public class ExampleLoadingSite : MonoBehaviour
{
    //You need a reference to UWB's WebBrowserClient, which is an object kept on BaseUwbClientManager
    //All of UWB's higher level components (such as WebBrowserUIBasic or WebBrowserUIFull) inherit from BaseUwbClientManager
    //so we can use that as the data type
    [SerializeField] //SerializeField allows us to set this in the editor
    private BaseUwbClientManager clientManager;
        
    private WebBrowserClient webBrowserClient;

    private void Start()
    {
        //You could also use Unity's GetComponent<BaseUwbClientManager>() method if this script exists on the same object.

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
