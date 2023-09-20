using Kuna.Net.Objects.V4.WS;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaTickerV4 : KunaOHLCVBase
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("percentagePriceChange")]
        public decimal PercentagePriceChange { get; set; }

        [JsonProperty("price")]
        public decimal Price { get => Close; set=> Close = value; }

        [JsonProperty("equivalentPrice")]
        public decimal? EquivalentPrice { get; set; }

        [JsonProperty("high")]
        public decimal H { set => High = value; }

        [JsonProperty("low")]
        public decimal L { set => Low = value; }

        [JsonProperty("baseVolume")]
        public decimal BaseVolume { set => Volume = value; }

        [JsonProperty("quoteVolume")]
        public decimal QuoteVolume { get; set; }

        [JsonProperty("bestBidPrice")]
        public decimal BestBidPrice { get; set; }

        [JsonProperty("bestAskPrice")]
        public decimal BestAskPrice { get; set; }

        [JsonProperty("priceChange")]
        public decimal PriceChange { get; set; }

        public override decimal Open { get => base.Open == 0 ? Price - PriceChange : base.Open; }
    }
}