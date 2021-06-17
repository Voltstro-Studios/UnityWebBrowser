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
            using EventReplier<PingEvent, OkResponse> eventReplier = new EventReplier<PingEvent, OkResponse>(port, incomingEvent => new OkResponse());
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
            OkResponse responseResponse = EventsSerializer.DeserializeEvent<OkResponse>(responseRawData);
            Assert.IsNotNull(responseResponse);
        }
        
        [Test]
        public void BasicUnionEventReplierTest()
        {
            const int port = 6677;
            using EventReplier<EngineActionEvent, EngineActionResponse> eventReplier = new EventReplier<EngineActionEvent, EngineActionResponse>(port, incomingEvent => new OkResponse());
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
            EngineActionResponse responseActionResponse = EventsSerializer.DeserializeEvent<EngineActionResponse>(responseRawData);
            Assert.IsNotNull(responseActionResponse);
            Assert.That(responseActionResponse.GetType(), Is.EqualTo(typeof(OkResponse)));
        }
    }
}