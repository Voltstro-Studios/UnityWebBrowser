using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VoltstroStudios.UnityWebBrowser.Shared;

namespace VoltstroStudios.UnityWebBrowser.Logging
{
    [Preserve]
    internal class JsonLogStructure
    {
        [JsonProperty("@t")] public DateTime Timestamp { get; set; }

        [JsonProperty("@m")] public string Message { get; set; }

        [JsonConverter(typeof(JsonLogSeverityConverter))]
        [JsonProperty("@l")]
        public LogSeverity Level { get; set; } = LogSeverity.Info;

        [JsonProperty("@x")] public string Exception { get; set; }

        [JsonProperty("@i")] public string EventId { get; set; }
    }
}