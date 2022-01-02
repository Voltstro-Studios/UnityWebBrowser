using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Logging
{
    public class JsonLogSeverityConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is string value)
            {
                if (value == "Error")
                    return LogSeverity.Error;
                else if (value == "Warning")
                    return LogSeverity.Warn;
                else if (value == "Debug")
                    return LogSeverity.Debug;
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(string))
                return true;

            return false;
        }

        public override bool CanWrite => false;

        public override bool CanRead => true;
    }
}