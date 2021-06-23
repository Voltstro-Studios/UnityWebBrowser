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
            
            //Setup test's ZMQ
            Utils.CreateZmq(ZSocketType.REP, port, true, out ZContext context, out ZSocket socket);

            //Create the event dispatcher
            EventDispatcher eventDispatcher = null;
            _ = Task.Run(() =>
            {
                eventDispatcher = new EventDispatcher(new TimeSpan(0, 0, 0, 4), port);
                eventDispatcher.DispatchEventsThread().RunSynchronously();
            });
            SpinWait.SpinUntil(() => eventDispatcher != null);
            
            //Send the event
            bool gotResponse = false;
            eventDispatcher.QueueEvent(new PingEvent(), frame =>
            {
                //We got a response
                gotResponse = true;
                EngineActionResponse responseEventDispatcher = EventsSerializer.DeserializeEvent<EngineActionResponse>(frame.Read());
                Assert.IsNotNull(responseEventDispatcher);
                Assert.That(responseEventDispatcher.GetType(), Is.EqualTo(typeof(OkResponse)));
                frame.Dispose();
            });

            //Get a event from the dispatcher
            byte[] requestData = socket.Receive();
            EngineActionEvent actionEvent = EventsSerializer.DeserializeEvent<EngineActionEvent>(requestData);
            Assert.IsNotNull(actionEvent);
            Assert.That(actionEvent.GetType(), Is.EqualTo(typeof(PingEvent)));
            
            //Respond
            EngineActionResponse response = new OkResponse();
            byte[] responseData = EventsSerializer.SerializeEvent<EngineActionResponse>(response);
            socket.Send(responseData);
            SpinWait.SpinUntil(() => gotResponse);
            
            eventDispatcher.Dispose();
            socket.Dispose();
            context.Dispose();
        }
    }
}