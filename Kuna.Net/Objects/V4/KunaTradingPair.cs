using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaTradingPair
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("baseAsset")]
        public KunaAsset BaseAsset { get; set; }

        [JsonProperty("quoteAsset")]
        public KunaAsset QuoteAsset { get; set; }

        [JsonProperty("tickerPriceChange")]
        public decimal TickerPriceChange { get; set; }
    }

    public class KunaAsset
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("precision")]
        public int Precision { get; set; }
    }
}