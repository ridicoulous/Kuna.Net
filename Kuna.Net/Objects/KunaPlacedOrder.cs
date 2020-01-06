using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public class KunaPlacedOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("side"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }

        [JsonProperty("ord_type"), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType OrderType { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("avg_price")]
        public decimal AvgPrice { get; set; }

        [JsonProperty("state"), JsonConverter(typeof(OrderStatusConverter))]
        public OrderState State { get; set; }

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
    }
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaPlacedOrderV3
    {
        /// <summary>
        /// The id of the order
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The group id of the order
        /// </summary>
        [ArrayProperty(1)]
        public long? GroupId { get; set; }

        /// <summary>
        /// The client order id
        /// </summary>
        [ArrayProperty(2)]
        public long? ClientOrderId { get; set; }

        /// <summary>
        /// The symbol of the order
        /// </summary>
        [ArrayProperty(3)]
        public string Symbol { get; set; } = "";

        /// <summary>
        /// The creation time of the order
        /// </summary>
        [ArrayProperty(4), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        /// <summary>
        /// The last update time
        /// </summary>
        [ArrayProperty(5), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampUpdated { get; set; }

        /// <summary>
        /// The amount left
        /// </summary>
        [ArrayProperty(6)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The original amount
        /// </summary>
        [ArrayProperty(7)]
        public decimal AmountOriginal { get; set; }

        /// <summary>
        /// The order type
        /// </summary>
        [ArrayProperty(8), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType Type { get; set; }

        /// <summary>
        /// The previous order type
        /// </summary>
        [ArrayProperty(9), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? TypePrevious { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(10)]
        public string PlaceHolder1 { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(11)]
        public string PlaceHolder2 { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(12)]
        public int? Flags { get; set; }


        /// <summary>
        /// The raw status string
        /// </summary>
        [ArrayProperty(13)]
        public string StatusString { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(14)]
        public string PlaceHolder3 { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        [ArrayProperty(15)]
        public string PlaceHolder4 { get; set; } = "";

        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(16)]
        public decimal Price { get; set; }

        /// <summary>
        /// The average price of the order (market orders)
        /// </summary>
        [ArrayProperty(17)]
        public decimal? PriceAverage { get; set; }
        [JsonIgnore]
        public OrderSide OrderSide => AmountOriginal > 0 ? OrderSide.Buy : OrderSide.Sell;


    }
}
