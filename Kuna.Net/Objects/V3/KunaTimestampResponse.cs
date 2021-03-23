using Newtonsoft.Json;

namespace Kuna.Net.Objects.V3
{
    public class KunaTimestampResponse
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set;}

        [JsonProperty("timestamp_miliseconds")]
        public long Timestamp_ms { get; set; }
    }
}