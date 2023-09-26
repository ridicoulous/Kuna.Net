using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaSocketTicker : KunaTicker
    {
        [JsonProperty("p")]
        public string P { set => Pair = value; }

        [JsonProperty("pc")]
        public decimal Pc { set => PriceChange = value; }

        [JsonProperty("pcp")]
        public decimal Pcp { set => PercentagePriceChange = value; }

        /// <summary>
        /// closing price of the previous frame, price of the last trade before OpenTime
        /// </summary>
        [JsonProperty("ftbp")]
        public decimal Price24HoursAgo { get; set; }

        [JsonProperty("lp")]
        public decimal Lp { set => Price = value;}

        [JsonProperty("lq")]
        public decimal LastTradedQty { get; set; }

        [JsonProperty("bbp")]
        public decimal Bbp { set => BestBidPrice = value; }

        [JsonProperty("bbq")]
        public decimal BestBidQuantity { get; set; }

        [JsonProperty("bap")]
        public decimal Bap { set => BestAskPrice = value; }

        [JsonProperty("baq")]
        public decimal BestAskQuantity { get; set; }

        [JsonProperty("ot")]
        public DateTimeOffset Ot { set => OpenTime = value; }

        [JsonProperty("ct")]
        public DateTimeOffset Ct { set => CloseTime = value; }

        [JsonProperty("n")]
        public int TradesAmount { get; set; }

        [JsonProperty("ttbav")]
        public decimal Ttbav { set => BaseVolume = value; }

        [JsonProperty("ttqav")]
        public decimal Ttqav { set => QuoteVolume = value; }
    }
}