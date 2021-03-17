using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonReader = Newtonsoft.Json.JsonReader;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using JsonWriter = Newtonsoft.Json.JsonWriter;

namespace CefBrowserProcess.EventData
{
	public static class EventDataParser
	{
		public static IEventData ReadData(string json)
		{
			return JsonConvert.DeserializeObject<IEventData>(json, new EventDataReader());
		}
	}

	public class EventDataReader : JsonConverter
	{
		public override bool CanRead { get; } = true;
		public override bool CanWrite { get; } = false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);
			if (jsonObject == null)
				throw new NullReferenceException("The JObject was null!");

			if (!jsonObject.ContainsKey("EventType"))
				throw new ArgumentNullException("EventType", "The event type key was not found!");

			// ReSharper disable PossibleNullReferenceException
			switch ((EventType)jsonObject.GetValue("EventType").ToObject<int>())
			{
				case EventType.Shutdown:
					return new ShutdownEvent();
				case EventType.Ping:
					return new PingEvent();
				case EventType.KeyboardEvent:
					return new KeyboardEvent
					{
						KeysUp = jsonObject.GetValue("KeysUp").ToObject<int[]>(),
						KeysDown = jsonObject.GetValue("KeysDown").ToObject<int[]>(),
						Chars = jsonObject.GetValue("Chars").ToObject<string>()
					};
				case EventType.MouseMoveEvent:
					return new MouseMoveEvent
					{
						MouseX = jsonObject.GetValue("MouseX").ToObject<int>(),
						MouseY = jsonObject.GetValue("MouseY").ToObject<int>()
					};
				case EventType.MouseClickEvent:
					return new MouseClickEvent
					{
						MouseX = jsonObject.GetValue("MouseX").ToObject<int>(),
						MouseY = jsonObject.GetValue("MouseY").ToObject<int>(),
						MouseClickCount = jsonObject.GetValue("MouseClickCount").ToObject<int>(),
						MouseClickType = (MouseClickType) jsonObject.GetValue("MouseClickType").ToObject<int>(),
						MouseEventType = (MouseEventType) jsonObject.GetValue("MouseEventType").ToObject<int>()
					};
				case EventType.MouseScrollEvent:
					return new MouseScrollEvent
					{
						MouseX = jsonObject.GetValue("MouseX").ToObject<int>(),
						MouseY = jsonObject.GetValue("MouseY").ToObject<int>(),
						MouseScroll = jsonObject.GetValue("MouseScroll").ToObject<int>()
					};
				case EventType.ButtonEvent:
					return new ButtonEvent
					{
						ButtonType = (ButtonType) jsonObject.GetValue("ButtonType").ToObject<int>(),
						UrlToNavigate = jsonObject.GetValue("UrlToNavigate").ToObject<string>()
					};
				default:
					throw new ArgumentOutOfRangeException();
				// ReSharper restore PossibleNullReferenceException
			}
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(IEventData) || objectType == typeof(KeyboardEvent) ||
			    objectType == typeof(PingEvent) || objectType == typeof(ShutdownEvent))
				return true;

			return false;
		}
	}
}