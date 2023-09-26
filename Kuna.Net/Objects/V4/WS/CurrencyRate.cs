using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaCurrencyRate
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("equivalent")]
        public IEnumerable<KunaCurrencyRateEquivalent> Equivalent { get; set; }
    }

    public class KunaCurrencyRateEquivalent
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }
}