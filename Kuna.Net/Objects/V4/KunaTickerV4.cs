using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaTickerV4
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("percentagePriceChange")]
        public decimal PercentagePriceChange { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("equivalentPrice")]
        public decimal? EquivalentPrice { get; set; }

        [JsonProperty("high")]
        public decimal High { get; set; }

        [JsonProperty("low")]
        public decimal Low { get; set; }

        [JsonProperty("baseVolume")]
        public decimal BaseVolume { get; set; }

        [JsonProperty("quoteVolume")]
        public decimal QuoteVolume { get; set; }

        [JsonProperty("bestBidPrice")]
        public decimal BestBidPrice { get; set; }

        [JsonProperty("bestAskPrice")]
        public decimal BestAskPrice { get; set; }

        [JsonProperty("priceChange")]
        public decimal PriceChange { get; set; }
    }
}