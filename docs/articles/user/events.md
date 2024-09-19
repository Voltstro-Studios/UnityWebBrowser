# Client Events

The <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> includes a fair number of [events](https://learn.microsoft.com/en-us/dotnet/standard/events/). You can use these events to listen to certain actions and perform an action. All events are fired on the main Unity thread.

You can subscribe to an event like so:

```csharp
public class UWBPrjDemo : MonoBehaviour
{
    [SerializeField]
    private WebBrowserUIBasic uiBasic;

    public void Start()
    {
        uiBasic.browserClient.OnClientInitialized += OnClientInitialized;
    }

    private void OnClientInitialized()
    {
        Debug.Log("BrowserClient initialized...");
    }
}
```

## OnClientInitialized

This event is invoked when UWB's <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> constructor has finished creating everything. This event doesn't mean that the engine is ready and connected.

## OnClientConnected

This event is invoked when UWB's <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient> has successfully connected back to the engine. At this point of stage the engine is ready, and you can invoke other web browser methods.

## OnUrlChanged

This event is invoked whenever the current URL of the web browser changes for any reason. This event may fire multiple times on the same URL. The full URL is provided in the event.

## OnLoadStart

This event is invoked whenever a page starts to load.

## OnLoadFinish

This event is invoked whenever a page finishes loading.

## OnLoadProgressChange

This event is invoked whenever the page's loading progress changes. The progress loading goes from a value of 0 to 1.

## OnFullscreen

This event is invoked whenever the browser goes fullscreen. E.G: The user fullscreens a YouTube video.

## OnPopup

This event is invoked whenever a popup opens. To know when that popup closes, use <xref:VoltstroStudios.UnityWebBrowser.Core.Popups.WebBrowserPopupInfo.OnDestroyed>.
