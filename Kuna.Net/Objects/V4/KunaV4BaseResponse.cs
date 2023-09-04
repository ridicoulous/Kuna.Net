using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    internal class KunaV4BaseResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}