using System;
using System.Threading;
using UnityEngine;
using ZeroMQ;

namespace UnityWebBrowser
{
    public class WebBrowserWorkerThread
    {
	    public WebBrowserWorkerThread(int errorsTillFail, string ipcEndpoint)
	    {
		    this.errorsTillFail = errorsTillFail;
		    this.ipcEndpoint = ipcEndpoint;

		    EventData = new EventData();
	    }

	    private readonly int errorsTillFail;
	    private readonly string ipcEndpoint;

	    private readonly object eventDataLock = new object();
	    private EventData eventData;

	    private readonly object pixelsLock = new object();
	    private byte[] pixels;

	    private int errorCount;
	    private bool isRunning;

	    public EventData EventData
	    {
		    get
		    {
			    lock (eventDataLock)
			    {
				    return eventData;
			    }
		    }
		    set
		    {
			    lock (eventDataLock)
			    {
				    eventData = value;
			    }
		    }
	    }

	    public byte[] Pixels
	    {
		    get
		    {
			    lock (pixelsLock)
			    {
				    return pixels;
			    }
		    }
		    set
		    {
			    lock (pixelsLock)
			    {
				    pixels = value;
			    }
		    }
	    }

		public void StartIpc()
	    {
		    //Start our client
		    using ZContext context = new ZContext();
		    using ZSocket requester = new ZSocket(context, ZSocketType.REQ)
		    {
			    SendTimeout = new TimeSpan(0, 0, 4),
			    ReceiveTimeout = new TimeSpan(0, 0, 4),
			    Linger = new TimeSpan(0, 0, 4)
		    };
		    requester.Connect(ipcEndpoint, out ZError error);

		    try
		    {
			    if (!Equals(error, ZError.None))
			    {
				    Debug.LogError("Server failed to start for some reason!");
				    return; 
			    }

			    isRunning = true;
			    errorCount = 0;

			    while (isRunning)
			    {
				    string data = JsonUtility.ToJson(EventData);

				    requester.Send(new ZFrame(data), out error);

				    EventData currentData = EventData;
				    currentData.LeftDown = false;
				    currentData.LeftUp = false;
				    currentData.RightDown = false;
				    currentData.RightUp = false;
				    currentData.Chars = "";
				    currentData.KeysDown = Array.Empty<int>();
				    currentData.KeysUp = Array.Empty<int>();
				    EventData = currentData;

				    if (!Equals(error, ZError.None))
				    {
					    errorCount++;
					    Debug.LogWarning($"Failed to send to server for some reason! {errorCount}");

					    if (errorCount >= errorsTillFail)
					    {
						    Debug.LogError($"Connection failed {errorCount} times! Quitting!");
							return;
					    }

					    continue;
				    }

				    using ZFrame reply = requester.ReceiveFrame(out error);

				    if (!Equals(error, ZError.None))
				    {
					    Debug.LogWarning("Failed to receive from server for some reason!");
					    continue; 
				    }

				    if (!isRunning)
					    break;

				    byte[] bytes = reply.Read();
				    Pixels = bytes;
			    }
		    }
		    catch (ThreadAbortException)
		    {
			    isRunning = false;
			    if(errorCount != errorsTillFail)
				    requester.Send(new ZFrame(JsonUtility.ToJson(new EventData
				    {
						Shutdown = true
				    })));
		    }
	    }
    }
}