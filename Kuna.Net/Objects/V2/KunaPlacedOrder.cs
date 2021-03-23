using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects.V2
{
    public class KunaPlacedOrderV2
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("side"), JsonConverter(typeof(OrderSideConverter))]
        public KunaOrderSideV2 Side { get; set; }

        [JsonProperty("ord_type"), JsonConverter(typeof(OrderTypeV2Converter))]
        public KunaOrderTypeV2 OrderType { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("avg_price")]
        public decimal AvgPrice { get; set; }

        [JsonProperty("state"), JsonConverter(typeof(OrderStatusV2Converter))]
        public KunaOrderStateV2 State { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        [JsonProperty("remaining_volume")]
        public decimal RemainingVolume { get; set; }

        [JsonProperty("executed_volume")]
        public decimal ExecutedVolume { get; set; }

        [JsonProperty("trades_count")]
        public int TradesCount { get; set; }
        [JsonProperty("trades")]
        public object[] Trades{ get; set; }
    }
}
