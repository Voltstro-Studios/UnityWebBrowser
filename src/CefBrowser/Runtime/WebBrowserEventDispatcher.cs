using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using UnityWebBrowser.EventData;
using ZeroMQ;

namespace UnityWebBrowser
{
    /// <summary>
    ///     Handles dispatching events to the browser process
    /// </summary>
    internal class WebBrowserEventDispatcher : IDisposable
    {
	    private bool isRunning;
	    private Thread eventDispatcherThread;

	    private readonly object eventsQueueLock = new object();
	    private readonly Queue<KeyValuePair<IEventData, Action<ZFrame>>> eventsQueue;

	    private int EventQueueCount
	    {
		    get
		    {
			    lock (eventsQueueLock)
			    {
				    return eventsQueue.Count;
			    }
		    }
	    }

	    private readonly ZContext context;

	    private readonly object requesterLock = new object();
	    private readonly ZSocket requester;

		/// <summary>
		///		Creates a new <see cref="WebBrowserEventDispatcher"/> instance
		/// </summary>
		/// <param name="timeOutTime"></param>
		/// <param name="port"></param>
        internal WebBrowserEventDispatcher(TimeSpan timeOutTime, int port = 5555)
        {
	        eventsQueue = new Queue<KeyValuePair<IEventData, Action<ZFrame>>>();

            //Setup ZMQ
	        context = new ZContext();
	        requester = new ZSocket(context, ZSocketType.REQ)
	        {
		        SendTimeout = timeOutTime,
		        ReceiveTimeout = timeOutTime,
		        Linger = timeOutTime
	        };

	        requester.Connect($"tcp://127.0.0.1:{port}", out ZError error);

	        if (!Equals(error, ZError.None))
		        throw new Exception("Failed to connect to server!");
        }

        /// <summary>
        ///     Queues an event
        /// </summary>
        /// <param name="eventData">The data to send</param>
        /// <param name="onReceive"><see cref="Action{T}"/> to be called when the <see cref="ZFrame"/> is received.
        /// BE-AWARE! This is called on the event dispatcher thread!</param>
        internal void QueueEvent(IEventData eventData, Action<ZFrame> onReceive = null)
        {
	        lock (eventsQueueLock)
	        {
		        eventsQueue.Enqueue(new KeyValuePair<IEventData, Action<ZFrame>>(eventData, onReceive));
	        }
        }

        private KeyValuePair<IEventData, Action<ZFrame>> DeQueueEvent()
        {
	        KeyValuePair<IEventData, Action<ZFrame>> data;

	        lock (eventsQueueLock)
	        {
		        data = eventsQueue.Dequeue();
	        }

	        return data;
        }

		/// <summary>
		///		Starts to dispatch events to the browser process
		/// </summary>
        internal void StartDispatchingEvents()
        {
	        eventDispatcherThread = new Thread(DispatchEventsThread) {Name = "Web Browser Event Dispatcher Thread"};
	        eventDispatcherThread.Start();
        }

        private void DispatchEventsThread()
        {
	        try
	        {
		        isRunning = true;

		        while (isRunning)
		        {
			        if(EventQueueCount == 0)
						continue;

					//Dequeue and send the event
			        KeyValuePair<IEventData, Action<ZFrame>> eventToSend = DeQueueEvent();
					SendEvent(eventToSend.Key);

					//Wait to receive the event
					try
					{
						ZFrame frame;
						lock (requesterLock)
						{
							frame = requester.ReceiveFrame();
						}
						
						eventToSend.Value?.Invoke(frame);
					}
					catch (ZException)
					{
					}
		        }
	        }
	        catch (ThreadAbortException)
	        {
		        isRunning = false;
	        }
        }

        private void SendEvent(IEventData eventData)
        {
	        lock (requesterLock)
	        {
		        requester.Send(new ZFrame(JsonConvert.SerializeObject(eventData)), out ZError error);

				if(error != null)
					if (!error.Equals(ZError.None))
					{
						//Error
					}
	        }
        }

        #region Destroy Methods

        ~WebBrowserEventDispatcher()
        {
            ReleaseResources();
        }

        public void Dispose()
        {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
	        isRunning = false;
            eventDispatcherThread.Abort();
			SendEvent(new ShutdownEvent());

			lock (requesterLock)
			{
				requester.Dispose();
			}
			context.Dispose();
        }
		
        #endregion
    }
}