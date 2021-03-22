using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects.V2
{
    public class KunaOhclvV2
    {
        public KunaOhclvV2(long timestamp, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Timestamp = new DateTime(1970, 1, 1).AddSeconds(timestamp);
        }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public DateTime Timestamp { get; set; }
    }
    internal class TradingViewOhclvV2
    {
        [JsonProperty("t")]
        public List<long> Timestamps { get; set; }

        [JsonProperty("h")]
        public List<decimal> Highs { get; set; }

        [JsonProperty("l")]
        public List<decimal> Lows { get; set; }

        [JsonProperty("o")]
        public List<decimal> Opens { get; set; }

        [JsonProperty("c")]
        public List<decimal> Closes { get; set; }

        [JsonProperty("v")]
        public List<decimal> Volumes { get; set; }

        [JsonProperty("s")]
        public string Status { get; set; }
    }
    
}
