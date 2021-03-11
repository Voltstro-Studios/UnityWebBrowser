using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityWebBrowser;
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
			switch ((EventType)jsonObject["EventType"].ToObject<int>())
			{
				case EventType.Shutdown:
					return new ShutdownEvent();
				case EventType.Ping:
					return new PingEvent();
				case EventType.KeyboardEvent:
					return new KeyboardEvent
					{
						KeysUp = jsonObject["KeysUp"].ToObject<int[]>(),
						KeysDown = jsonObject["KeysDown"].ToObject<int[]>(),
						Chars = jsonObject["Chars"].ToObject<string>()
					};
				case EventType.MouseMoveEvent:
					return new MouseMoveEvent()
					{
						MouseX = jsonObject["MouseX"].ToObject<int>(),
						MouseY = jsonObject["MouseY"].ToObject<int>()
					};
				default:
					throw new ArgumentOutOfRangeException();
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