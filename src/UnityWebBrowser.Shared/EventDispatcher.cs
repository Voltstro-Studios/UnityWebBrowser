using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace UnityWebBrowser.Shared
{
	/// <summary>
	///     Handles dispatching events to the browser process
	/// </summary>
	public class EventDispatcher<TEvent, TResponseEvent> : IDisposable
    {
        private readonly ZContext context;
        private readonly Queue<KeyValuePair<TEvent, Action<TResponseEvent>>> eventsQueue;

        private readonly object eventsQueueLock = new object();
        private readonly ZSocket requester;

        private readonly object requesterLock = new object();

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        /// <summary>
        ///     Creates a new <see cref="EventDispatcher{TEvent,TResponseEvent}" /> instance
        /// </summary>
        /// <param name="timeOutTime"></param>
        /// <param name="port"></param>
        public EventDispatcher(TimeSpan timeOutTime, int port = 5555)
        {
            eventsQueue = new Queue<KeyValuePair<TEvent, Action<TResponseEvent>>>();

            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            
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
        public void QueueEvent(TEvent eventData, Action<TResponseEvent> onReceive = null)
        {
            lock (eventsQueueLock)
            {
                eventsQueue.Enqueue(new KeyValuePair<TEvent, Action<TResponseEvent>>(eventData, onReceive));
            }
        }

        /// <summary>
        ///     Sends an <see cref="TEvent"/> directly, this will not handle receiving events back!
        /// </summary>
        /// <param name="eventData"></param>
        public void SendEvent(TEvent eventData)
        {
            lock (requesterLock)
            {
                byte[] data = EventsSerializer.SerializeEvent<TEvent>(eventData);
                requester.Send(new ZFrame(data), out ZError error);

                if (error == null) return;
                if (!error.Equals(ZError.None))
                {
                    //Error
                }
            }
        }
        
        private KeyValuePair<TEvent, Action<TResponseEvent>> DeQueueEvent()
        {
            KeyValuePair<TEvent, Action<TResponseEvent>> data;

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
            _ = Task.Run(DispatchEventsThread);
        }

        internal Task DispatchEventsThread()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (EventQueueCount == 0)
                    continue;

                //Dequeue and send the event
                KeyValuePair<TEvent, Action<TResponseEvent>> eventToSend = DeQueueEvent();
                SendEvent(eventToSend.Key);

                //Wait to receive the event
                try
                {
                    ZFrame frame;
                    lock (requesterLock)
                    {
                        frame = requester.ReceiveFrame();
                    }

                    //Should we bother to put in the effort in deserializing the event
                    if (eventToSend.Value != null)
                    {
                        TResponseEvent responseEvent = EventsSerializer.DeserializeEvent<TResponseEvent>(frame.Read());
                        eventToSend.Value.Invoke(responseEvent);
                    }
                    
                    frame.Dispose();
                }
                catch (ZException)
                {
                }
            }
            
            return Task.CompletedTask;
        }

        #region Destroy Methods

        ~EventDispatcher()
        {
            ReleaseResources();
        }

        /// <summary>
        ///     Cancels the thread dispatching events
        /// </summary>
        public void CancelQueueThead()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Destroys 
        /// </summary>
        public void Dispose()
        {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
            CancelQueueThead();
            lock (requesterLock)
            {
                requester.Dispose();
            }

            context.Dispose();
        }

        #endregion
    }
}