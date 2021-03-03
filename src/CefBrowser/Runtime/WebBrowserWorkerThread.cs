using System;
using System.Threading;
using UnityEngine;
using ZeroMQ;

namespace UnityWebBrowser
{
	/// <summary>
	///		Handles communication between Unity and the CEF process
	/// </summary>
    internal sealed class WebBrowserWorkerThread
    {
		/// <summary>
		///		Creates a new <see cref="WebBrowserWorkerThread"/> instance
		/// </summary>
		/// <param name="errorsTillFail"></param>
		/// <param name="ipcEndpoint"></param>
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

		/// <summary>
		///		The the communication between Unity and the CEF process
		///		<para>The CEF process is assumed to be running before calling this!</para>
		/// </summary>
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
					//Convert our event data to json and send it
				    string data = JsonUtility.ToJson(EventData);
				    requester.Send(new ZFrame(data), out error);

					//Reset our even data
				    EventData currentData = EventData;
				    currentData.LeftDown = false;
				    currentData.LeftUp = false;
				    currentData.RightDown = false;
				    currentData.RightUp = false;
				    currentData.Chars = "";
				    currentData.KeysDown = Array.Empty<int>();
				    currentData.KeysUp = Array.Empty<int>();
				    EventData = currentData;

					//Check errors
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

					//Get our reply
				    using ZFrame reply = requester.ReceiveFrame(out error);

					//Check errors
				    if (!Equals(error, ZError.None))
				    {
					    Debug.LogWarning("Failed to receive from server for some reason!");
					    continue; 
				    }

				    if (!isRunning)
					    break;

					//Read pixel data
				    byte[] bytes = reply.Read();
				    Pixels = bytes;
			    }
		    }
		    catch (ThreadAbortException)
		    {
				//Shutdown
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