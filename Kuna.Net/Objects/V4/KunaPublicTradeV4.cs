using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaPublicTradeV4
    {
        private decimal quantity;
        private decimal price;

        /// <summary>
        /// Unique identifier of a trade
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("pair")]
        public string Pair { get; set; }

        /// <summary>
        /// Quote asset quantity
        /// </summary>
        [JsonProperty("quoteQuantity")]
        public decimal QuoteQuantity { get; set; }
        /// <summary>
        /// Traded quantity
        /// </summary>
        [JsonProperty("quantity")]
        public decimal Quantity { get => quantity; set => quantity = value; }

        /// <summary>
        /// same as Quantity
        /// </summary>
        [JsonProperty("matchQuantity")]
        public decimal MatchQuantity {set => quantity = value; }

        /// <summary>
        /// Traded pricem, the same as Price
        /// </summary>
        [JsonProperty("matchPrice")]
        public decimal MatchPrice { set => price = value; }

        /// <summary>
        /// Traded price
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get => price; set => price = value; }

        /// <summary>
        /// Date-time of trade execution, UTC
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("side")]
        public KunaOrderSideV4 Side { get; set; }
    }

}