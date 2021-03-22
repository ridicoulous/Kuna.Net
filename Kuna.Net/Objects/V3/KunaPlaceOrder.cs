using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;


namespace Kuna.Net.Objects.V3
{

    [JsonConverter(typeof(ArrayConverter))]
    public class KunaPlacedOrder
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