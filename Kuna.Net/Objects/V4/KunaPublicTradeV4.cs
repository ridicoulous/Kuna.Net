using System;
using Kuna.Net.Objects.V3;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaPublicTradeV4
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("quoteQuantity")]
        public decimal QuoteQuantity { get; set; }

        [JsonProperty("matchPrice")]
        public decimal MatchPrice { get; set; }

        [JsonProperty("matchQuantity")]
        public decimal MatchQuantity { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("side")]
        public KunaOrderSideV4 Side { get; set; }
    }

}