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

        /// <summary>
        /// the same as Currency, just for compability with socket update event object
        /// </summary>
        [JsonProperty("code")]
        public string C { set => Currency = value; }
    }
}