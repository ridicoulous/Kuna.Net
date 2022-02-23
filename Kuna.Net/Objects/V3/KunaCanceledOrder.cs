using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Kuna.Net.Objects.V3
{
    public  class KunaCanceledOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("side"),JsonConverter(typeof(OrderSideConverter))]
        public KunaOrderSide Side { get; set; }

        [JsonProperty("type"), JsonConverter(typeof(OrderTypeConverter))]
        public KunaOrderType Type { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("avg_execution_price")]
        public decimal AvgExecutionPrice { get; set; }

        [JsonProperty("state"), JsonConverter(typeof(OrderStatusV2Converter))]
        public KunaOrderStatus Status { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("timestamp"),JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("original_amount")]
        public decimal OriginalAmount { get; set; }

        [JsonProperty("remaining_amount")]
        public decimal RemainingAmount { get; set; }

        [JsonProperty("executed_amount")]
        public decimal ExecutedAmount { get; set; }

        [JsonProperty("is_cancelled")]
        public bool? IsCancelled { get; set; }

        [JsonProperty("is_hidden")]
        public bool? IsHidden { get; set; }

        [JsonProperty("is_live")]
        public bool? IsLive { get; set; }

        [JsonProperty("was_forced")]
        public bool? WasForced { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        public string CommonId => Id.ToString();
    }

}
