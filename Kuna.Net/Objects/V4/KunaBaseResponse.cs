using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    internal class KunaBaseResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}