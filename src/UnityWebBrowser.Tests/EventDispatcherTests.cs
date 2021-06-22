using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using UnityWebBrowser.Shared.Events.EngineActionResponse;
using ZeroMQ;

namespace UnityWebBrowser.Tests
{
    public class EventDispatcherTests
    {
        [Test]
        public void BasicEventDispatcherTest()
        {
            const int port = 8732;
            
            using ZContext context = new ZContext();
            using ZSocket socket = new ZSocket(context, ZSocketType.REP);
            socket.Bind($"tcp://127.0.0.1:{port}", out ZError error);
            Assert.AreEqual(ZError.None, error);

            EventDispatcher eventDispatcher = null;
            _ = Task.Run(() =>
            {
                eventDispatcher = new EventDispatcher(new TimeSpan(0, 0, 0, 4), port);
                eventDispatcher.DispatchEventsThread().RunSynchronously();
            });

            _ = Task.Run(() =>
            {
                using ZFrame request = socket.ReceiveFrame();
                EngineActionEvent actionEvent = EventsSerializer.DeserializeEvent<EngineActionEvent>(request.Read());
                Assert.IsNotNull(actionEvent);
                Assert.That(actionEvent.GetType(), Is.EqualTo(typeof(PingEvent)));
                
                socket.Send(new ZFrame(EventsSerializer.SerializeEvent<EngineActionResponse>(new OkResponse())));
            });
            
            Thread.Sleep(100);

            bool gotResponse = false;
            eventDispatcher.QueueEvent(new PingEvent(), frame =>
            {
                gotResponse = true;
                EngineActionResponse response = EventsSerializer.DeserializeEvent<EngineActionResponse>(frame.Read());
                Assert.IsNotNull(response);
                Assert.That(response.GetType(), Is.EqualTo(typeof(OkResponse)));
                frame.Dispose();
            });
            
            

            SpinWait.SpinUntil(() => gotResponse);
            
            eventDispatcher.Dispose();
        }
    }
}