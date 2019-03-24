using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Kuna.Net.Objects
{
    public class KunaTickerInfo
    {
        [JsonProperty("at"), JsonConverter(typeof(TimestampConverter))]
        public DateTime ServerTime { get; set; }
        [JsonProperty("ticker")]
        public KunaTicker Ticker { get; set; }
    }
    public class KunaTicker
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
