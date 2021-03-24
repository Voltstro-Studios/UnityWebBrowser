namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
	public interface IEventData
    {
	    public EventType EventType { get; set; }
    }
}