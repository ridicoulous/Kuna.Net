using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kuna.Net.Objects.V4
{
    public class KunaTimestampResponse
    {
        [JsonProperty("timestamp"), JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CurrentTime { get; set;}

        [JsonProperty("timestamp_miliseconds")]
        public long Timestamp_ms { get; set; }
    }
}