using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects.V2
{
    public class KunaTraidingPairV2
    {
        [JsonProperty("id")]
        public string Pair { get; set; }

        [JsonProperty("base_unit")]
        public string Base { get; set; }

        [JsonProperty("quote_unit")]
        public string Quote { get; set; }

        [JsonProperty("base_precision")]
        public int BasePrecision { get; set; }

        [JsonProperty("quote_precision")]
        public int QuotePrecision { get; set; }

        [JsonProperty("display_precision")]
        public int DisplayPrecision { get; set; }

        [JsonProperty("price_change")]
        public decimal PriceChange { get; set; }
    }
}
