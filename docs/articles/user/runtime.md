# Runtime Creation

The <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> can be created manually at runtime without the usage of <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIBasic> or <xref:VoltstroStudios.UnityWebBrowser.WebBrowserUIFull>.

When instantiating the class, you will need to set all required properties yourself.

> [!NOTE]
> <xref:VoltstroStudios.UnityWebBrowser.Communication.CommunicationLayer> and <xref:VoltstroStudios.UnityWebBrowser.Core.Engines.Engine> objects are Unity's [ScriptableObject](https://docs.unity3d.com/2021.3/Documentation/Manual/class-ScriptableObject.html). The associated properties on <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> must be set to a ScriptableObject instance, or created at runtime through the [`ScriptableObject.CreateInstance()`](https://docs.unity3d.com/2021.3/Documentation/ScriptReference/ScriptableObject.CreateInstance.html) method.

There is a sample script that creates everything including the canvas, the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.engine>, the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.communicationLayer> and the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> instance it self. You can import the script straight into your Unity project via the sample sections of the UWB package, or you can [view the script in GitHub](https://github.com/Voltstro-Studios/UnityWebBrowser/blob/master/src/Packages/UnityWebBrowser/Samples~/Runtime/Scripts/UWBRuntime.cs).

## Headless Mode

<xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> constructor includes a `headless` parameter. When set to `true`, UWB will run in headless mode.

Headless mode will not create a <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.BrowserTexture>, and it will not run UWB's internal pixel data thread. All other methods that the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> has are available.
