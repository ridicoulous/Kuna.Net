using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaTradeV4
    {
        /// <summary>
        /// Unique identifier of a trade
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Unique identifier of an associated order
        /// </summary>
        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

        [JsonProperty("pair")]
        public string Pair { get; set; }
        /// <summary>
        /// Traded quantity
        /// </summary>
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Traded price
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Various fees for Makers and Takers; "Market" orders are always `true`
        /// </summary>
        [JsonProperty("isTaker")]
        public bool IsTaker { get; set; }
        /// <summary>
        /// Exchange commission fee
        /// </summary>
        [JsonProperty("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Currency of the commission
        /// </summary>
        [JsonProperty("feeCurrency")]
        public string FeeCurrency { get; set; }
        /// <summary>
        /// Buy or sell the base asset
        /// </summary>
        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }
        /// <summary>
        /// Quote asset quantity
        /// </summary>
        [JsonProperty("quoteQuantity")]
        public decimal QuoteQuantity { get; set; }
        /// <summary>
        /// Date-time of trade execution, UTC
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}