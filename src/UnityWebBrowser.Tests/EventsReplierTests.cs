using System.Threading.Tasks;
using NUnit.Framework;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineActions;
using UnityWebBrowser.Shared.Events.EngineEvents;
using ZeroMQ;

namespace UnityWebBrowser.Tests
{
    public class EventsReplierTests
    {
        [Test]
        public void BasicEventReplierTest()
        {
            const int port = 5566;
            using EventReplier<PingEvent, OkEvent> eventReplier = new EventReplier<PingEvent, OkEvent>(port, incomingEvent => new OkEvent());
            // ReSharper disable once AccessToDisposedClosure
            _ = Task.Run(() => eventReplier.HandleEventsLoop());

            using ZContext context = new ZContext();
            using ZSocket socket = new ZSocket(context, ZSocketType.REQ);
            socket.Connect($"tcp://127.0.0.1:{port}", out ZError error);
            Assert.AreEqual(ZError.None, error);

            byte[] okEventData = EventsSerializer.SerializeEvent(new PingEvent());
            socket.Send(new ZFrame(okEventData), out error);
            Assert.AreEqual(ZError.None, error);

            using ZFrame response = socket.ReceiveFrame(out error);
            Assert.AreEqual(ZError.None, error);

            byte[] responseRawData = response.Read();
            OkEvent responseEvent = EventsSerializer.DeserializeEvent<OkEvent>(responseRawData);
            Assert.IsNotNull(responseEvent);
        }
        
        [Test]
        public void BasicUnionEventReplierTest()
        {
            const int port = 6677;
            using EventReplier<EngineActionEvent, EngineEvent> eventReplier = new EventReplier<EngineActionEvent, EngineEvent>(port, incomingEvent => new OkEvent());
            // ReSharper disable once AccessToDisposedClosure
            _ = Task.Run(() => eventReplier.HandleEventsLoop());

            using ZContext context = new ZContext();
            using ZSocket socket = new ZSocket(context, ZSocketType.REQ);
            socket.Connect($"tcp://127.0.0.1:{port}", out ZError error);
            Assert.AreEqual(ZError.None, error);

            byte[] okEventData = EventsSerializer.SerializeEvent<EngineActionEvent>(new PingEvent());
            socket.Send(new ZFrame(okEventData), out error);
            Assert.AreEqual(ZError.None, error);

            using ZFrame response = socket.ReceiveFrame(out error);
            Assert.AreEqual(ZError.None, error);

            byte[] responseRawData = response.Read();
            EngineEvent responseEvent = EventsSerializer.DeserializeEvent<EngineEvent>(responseRawData);
            Assert.IsNotNull(responseEvent);
            Assert.That(responseEvent.GetType(), Is.EqualTo(typeof(OkEvent)));
        }
    }
}