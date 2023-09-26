using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaMiniTicker : KunaOHLCVBase
    {
        [JsonProperty("p")]
        public string Pair { get; set; }

        [JsonProperty("n")]
        public int TradesAmount { get; set; }

        [JsonProperty("ttbav")]
        public decimal BaseVolume { set => Volume = value; }

        [JsonProperty("ttqav")]
        public decimal QuoteVolume { get; set; }
    }
}