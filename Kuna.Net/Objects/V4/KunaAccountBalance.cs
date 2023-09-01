using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaAccountBalance
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("lockBalance")]
        public decimal LockBalance { get; set; }
    }
}