using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class SocketTicker : KunaTickerV4
    {
        [JsonProperty("p")]
        public string P { set => Pair = value; }

        [JsonProperty("pc")]
        public decimal Pc { set => PriceChange = value; }

        [JsonProperty("pcp")]
        public decimal Pcp { set => PercentagePriceChange = value; }

        [JsonProperty("ftbp")]
        public decimal Price24HoursAgo { get; set; }

        [JsonProperty("lp")]
        public decimal Lp { set => Price = value;}

        [JsonProperty("lq")]
        public decimal LastTradedQty { get; set; }

        [JsonProperty("bbp")]
        public decimal Bbp { set => BestBidPrice = value; }

        [JsonProperty("bbq")]
        public decimal Bbq { get; set; }

        [JsonProperty("bap")]
        public string Bap { get; set; }

        [JsonProperty("baq")]
        public string Baq { get; set; }

        [JsonProperty("o")]
        public string O { get; set; }

        [JsonProperty("h")]
        public string H { get; set; }

        [JsonProperty("l")]
        public string L { get; set; }

        [JsonProperty("ot")]
        public DateTimeOffset Ot { get; set; }

        [JsonProperty("ct")]
        public DateTimeOffset Ct { get; set; }

        [JsonProperty("n")]
        public int N { get; set; }


        [JsonProperty("ttbav")]
        public string Ttbav { get; set; }

        [JsonProperty("ttqav")]
        public string Ttqav { get; set; }
    }
}