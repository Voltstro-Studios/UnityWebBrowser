using System.Threading.Tasks;
using NUnit.Framework;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using UnityWebBrowser.Shared.Events.EngineActionResponse;
using ZeroMQ;

namespace UnityWebBrowser.Tests
{
    public class EventsReplierTests
    {
        [Test]
        public void BasicEventReplierTest()
        {
            const int port = 5566;
            
            //Create event replier
            using EventReplier<PingEvent, OkResponse> eventReplier = new(port, _ => new OkResponse());
            // ReSharper disable once AccessToDisposedClosure
            _ = Task.Run(() => eventReplier.HandleEventsLoop());
            
            //Setup test's zmq
            Utils.CreateZmq(ZSocketType.REQ, port, false, out ZContext context, out ZSocket socket);

            //Create ping event and serialize it
            byte[] okEventData = EventsSerializer.SerializeEvent(new PingEvent());
            
            //Send it
            socket.Send(okEventData);

            //Get the response back from the event replier
            byte[] responseRawData = socket.Receive();
            OkResponse responseResponse = EventsSerializer.DeserializeEvent<OkResponse>(responseRawData);
            Assert.IsNotNull(responseResponse);
            
            socket.Dispose();
            context.Dispose();
        }
        
        [Test]
        public void BasicUnionEventReplierTest()
        {
            const int port = 6677;
            
            using EventReplier<EngineActionEvent, EngineActionResponse> eventReplier = new(port, _ => new OkResponse());
            // ReSharper disable once AccessToDisposedClosure
            _ = Task.Run(() => eventReplier.HandleEventsLoop());

            //Setup test's zmq
            Utils.CreateZmq(ZSocketType.REQ, port, false, out ZContext context, out ZSocket socket);

            //Get the response
            byte[] okEventData = EventsSerializer.SerializeEvent<EngineActionEvent>(new PingEvent());
            socket.Send(okEventData);

            //Check the type
            byte[] responseRawData = socket.Receive();
            EngineActionResponse responseActionResponse = EventsSerializer.DeserializeEvent<EngineActionResponse>(responseRawData);
            Assert.IsNotNull(responseActionResponse);
            Assert.That(responseActionResponse.GetType(), Is.EqualTo(typeof(OkResponse)));
            
            socket.Dispose();
            context.Dispose();
        }
    }
}