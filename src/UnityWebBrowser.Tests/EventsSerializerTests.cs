using NUnit.Framework;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineActions;

namespace UnityWebBrowser.Tests
{
    public class EventsSerializerTests
    {
        [Test]
        public void BasicSerializationTest()
        {
            byte[] data = EventsSerializer.SerializeEvent(new PingEvent());

            PingEvent pingEvent = EventsSerializer.DeserializeEvent<PingEvent>(data);
        }

        [Test]
        public void BasicUnionSerializationTest()
        {
            byte[] data = EventsSerializer.SerializeEvent<EngineActionEvent>(new PingEvent());

            EngineActionEvent engineEvent = EventsSerializer.DeserializeEvent<EngineActionEvent>(data);
            Assert.That(engineEvent.GetType(), Is.EqualTo(typeof(PingEvent)));
        }
    }
}