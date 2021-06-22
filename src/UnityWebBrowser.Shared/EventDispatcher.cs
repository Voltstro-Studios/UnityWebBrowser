using System;
using System.Collections.Generic;
using System.Threading;
using UnityWebBrowser.Shared.Events.EngineAction;
using ZeroMQ;

namespace UnityWebBrowser.Shared
{
	/// <summary>
	///     Handles dispatching events to the browser process
	/// </summary>
	public class EventDispatcher : IDisposable
    {
        private readonly ZContext context;
        private readonly Queue<KeyValuePair<EngineActionEvent, Action<ZFrame>>> eventsQueue;

        private readonly object eventsQueueLock = new object();
        private readonly ZSocket requester;

        private readonly object requesterLock = new object();
        private Thread eventDispatcherThread;
        private bool isRunning;

        /// <summary>
        ///     Creates a new <see cref="EventDispatcher" /> instance
        /// </summary>
        /// <param name="timeOutTime"></param>
        /// <param name="port"></param>
        public EventDispatcher(TimeSpan timeOutTime, int port = 5555)
        {
            eventsQueue = new Queue<KeyValuePair<EngineActionEvent, Action<ZFrame>>>();
            
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

        /// <summary>
        ///     Queues an event
        /// </summary>
        /// <param name="eventData">The data to send</param>
        /// <param name="onReceive">
        ///     <see cref="Action{T}" /> to be called when the <see cref="ZFrame" /> is received.
        ///     BE-AWARE! This is called on the event dispatcher thread!
        /// </param>
        public void QueueEvent(EngineActionEvent eventData, Action<ZFrame> onReceive = null)
        {
            lock (eventsQueueLock)
            {
                eventsQueue.Enqueue(new KeyValuePair<EngineActionEvent, Action<ZFrame>>(eventData, onReceive));
            }
        }

        private KeyValuePair<EngineActionEvent, Action<ZFrame>> DeQueueEvent()
        {
            KeyValuePair<EngineActionEvent, Action<ZFrame>> data;

            lock (eventsQueueLock)
            {
                data = eventsQueue.Dequeue();
            }

            return data;
        }

        /// <summary>
        ///     Starts to dispatch events to the browser process
        /// </summary>
        public void StartDispatchingEvents()
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
                    if (EventQueueCount == 0)
                        continue;

                    //Dequeue and send the event
                    KeyValuePair<EngineActionEvent, Action<ZFrame>> eventToSend = DeQueueEvent();
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

        private void SendEvent(EngineActionEvent eventData)
        {
            lock (requesterLock)
            {
                byte[] data = EventsSerializer.SerializeEvent(eventData);
                requester.Send(new ZFrame(data), out ZError error);

                if (error != null)
                    if (!error.Equals(ZError.None))
                    {
                        //Error
                    }
            }
        }

        #region Destroy Methods

        ~EventDispatcher()
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