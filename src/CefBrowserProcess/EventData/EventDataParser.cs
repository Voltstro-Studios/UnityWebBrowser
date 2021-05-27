using System;
using System.Linq;
using System.Text.Json;
using CefBrowserProcess.Core;
using UnityWebBrowser.EventData;

namespace CefBrowserProcess.EventData
{
	public static class EventDataParser
	{
		public static IEventData ReadData(string json)
		{
			JsonElement.ObjectEnumerator document = JsonDocument.Parse(json).RootElement.EnumerateObject();
			try
			{
				JsonProperty eventType = document.Single(x => x.NameEquals("EventType"));
				if (eventType.Value.ValueKind != JsonValueKind.Number)
				{
					Logger.Error("EventType is not the correct type!");
					return null;
				}

				if (!eventType.Value.TryGetInt32(out int value))
				{
					Logger.Error("Failed to get EventType value as a number!");
					return null;
				}

				return (EventType) value switch
				{
					EventType.Ping => new PingEvent(),
					EventType.Shutdown => new ShutdownEvent(),
					EventType.ButtonEvent => JsonSerializer.Deserialize<ButtonEvent>(json),
					EventType.ExecuteJsEvent => JsonSerializer.Deserialize<ExecuteJsEvent>(json),
					EventType.KeyboardEvent => JsonSerializer.Deserialize<KeyboardEvent>(json),
					EventType.LoadHtmlEvent => JsonSerializer.Deserialize<LoadHtmlEvent>(json),
					EventType.MouseClickEvent => JsonSerializer.Deserialize<MouseClickEvent>(json),
					EventType.MouseMoveEvent => JsonSerializer.Deserialize<MouseMoveEvent>(json),
					EventType.MouseScrollEvent => JsonSerializer.Deserialize<MouseScrollEvent>(json),
					_ => throw new ArgumentOutOfRangeException()
				};
			}
			catch(InvalidOperationException)
			{
				Console.WriteLine("There is either no or multiple instances of EventType!");
				return null;
			}
		}
	}
}