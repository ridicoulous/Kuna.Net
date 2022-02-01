using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Kuna.Net.Objects.V2
{
    public class KunaTickerInfoV2
    {
        [JsonProperty("at"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime ServerTime { get; set; }
        [JsonProperty("ticker")]
        public KunaTickerV2 Ticker { get; set; }
    }
    public class KunaTickerV2
    {
        [JsonProperty("buy")]
        public string Buy { get; set; }

        [JsonProperty("sell")]
        public string Sell { get; set; }

        [JsonProperty("low")]
        public string Low { get; set; }

        [JsonProperty("high")]
        public string High { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }

        [JsonProperty("vol")]
        public string Vol { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }
    }
}
