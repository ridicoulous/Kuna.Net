using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Kuna.Net.Objects
{
    public class KunaTrade
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("price"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }

        [JsonProperty("volume"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Volume { get; set; }

        [JsonProperty("funds"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal TradeSumm { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("created_at"), JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("trend"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide TradeType { get; set; }
        public bool IsMaker()
        {
            if (String.IsNullOrEmpty(Side))
            {
                return false;
            }
            if (Side == "ask")
            {
                return TradeType == OrderSide.Buy;
            }
            if (Side == "bid")
            {
                return TradeType == OrderSide.Sell;
            }
            else return false;

        }

    }

    /// <summary>
    /// Details of a trade
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaTrade3
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The pair the trade is for
        /// </summary>
        [ArrayProperty(1)]
        public string Pair { get; set; } = "";

        /// <summary>
        /// The time the trade was created
        /// </summary>
        [ArrayProperty(2), JsonConverter(typeof(TimestampConverter))]
        public DateTime TimestampCreated { get; set; }

        /// <summary>
        /// The id of the order
        /// </summary>
        [ArrayProperty(3)]
        public long OrderId { get; set; }

        /// <summary>
        /// The executed amount
        /// </summary>
        [ArrayProperty(4)]
        public decimal ExecutedAmount { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        [ArrayProperty(5)]
        public decimal ExecutedPrice { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType? OrderType { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        [ArrayProperty(7)]
        public decimal? OrderPrice { get; set; }

        /// <summary>
        /// If was maker
        /// </summary>
        [ArrayProperty(8), JsonConverter(typeof(BoolToIntConverter))]
        public bool? Maker { get; set; }

        /// <summary>
        /// The fee
        /// </summary>
        [ArrayProperty(9)]
        public decimal Fee { get; set; }

        /// <summary>
        /// The currency the fee is in
        /// </summary>
        [ArrayProperty(10)]
        public string FeeCurrency { get; set; } = "";
    }


}
