using System;
using System.Threading.Tasks;
using ZeroMQ;

namespace UnityWebBrowser.Shared
{
    public class EventReplier<TEvent, TResponseEvent> : IDisposable
    {
        public delegate TResponseEvent EventReceived(TEvent engineActionEvent);
        
        private readonly EventReceived onEventReceived;
        private readonly ZContext context;
        private readonly ZSocket socket;

        private bool isRunning;

        public EventReplier(int port, EventReceived eventReceived)
        {
            context = new ZContext();
            socket = new ZSocket(context, ZSocketType.REP);
            socket.Bind($"tcp://127.0.0.1:{port}");
            onEventReceived = eventReceived;
        }

        public Task HandleEventsLoop()
        {
            isRunning = true;
            while (isRunning)
            {
                using ZFrame receivedFrame = socket.ReceiveFrame();
                byte[] receivedRawData = receivedFrame.Read();
                TEvent engineActionEvent = EventsSerializer.DeserializeEvent<TEvent>(receivedRawData);
                TResponseEvent response = onEventReceived(engineActionEvent);
                byte[] responseRawData = EventsSerializer.SerializeEvent<TResponseEvent>(response);
                socket.Send(new ZFrame(responseRawData));
            }
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            isRunning = false;
            socket?.Dispose();
            context?.Dispose();
        }
    }
}