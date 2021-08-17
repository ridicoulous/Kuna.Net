using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;


namespace Kuna.Net.Objects.V3
{

    [JsonConverter(typeof(ArrayConverter))]
    public class KunaPlacedOrder : ICommonOrder
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

        [ArrayProperty(6)]
        public decimal AmountLeft { get; set; }
        /// <summary>
        /// The original amount
        /// </summary>
        [ArrayProperty(7)]
        public decimal AmountPlaced { get; set; }
        /// <summary>
        /// The amount left
        /// </summary>
   

        [JsonIgnore]
        public decimal AmountExecuted => Math.Abs(AmountPlaced) - AmountLeft;
        /// <summary>
        /// The order type
        /// </summary>
        [ArrayProperty(8), JsonConverter(typeof(OrderTypeConverter))]
        public KunaOrderType Type { get; set; }

        /// <summary>
        /// The previous order type
        /// </summary>
        [ArrayProperty(9), JsonConverter(typeof(OrderTypeConverter))]
        public KunaOrderType? TypePrevious { get; set; }
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
        public string StatusString { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [ArrayProperty(13), JsonConverter(typeof(KunaOrderStatusConverter))]
        public KunaOrderStatus Status { get; set; }
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
        public KunaOrderSide OrderSide => AmountPlaced > 0 ? KunaOrderSide.Buy : KunaOrderSide.Sell;

        [JsonIgnore]
        public bool IsPartiallyFilled => AmountLeft > 0 && AmountExecuted > 0 ? true: false;
        public string CommonId => Id.ToString();

        public string CommonSymbol => Symbol;

        public decimal CommonPrice => Price == 0 ? PriceAverage ?? 0 : Price;

        public decimal CommonQuantity => Math.Abs(AmountPlaced);

        public IExchangeClient.OrderStatus CommonStatus => Status switch 
        {
            KunaOrderStatus.Active => IExchangeClient.OrderStatus.Active,
            KunaOrderStatus.Canceled => IExchangeClient.OrderStatus.Canceled,
            KunaOrderStatus.Filled => IExchangeClient.OrderStatus.Filled,
            _=> throw new NotImplementedException("Undefined order status")
        };

        public bool IsActive => CommonStatus == IExchangeClient.OrderStatus.Active;

        public IExchangeClient.OrderSide CommonSide => AmountPlaced > 0 ? IExchangeClient.OrderSide.Buy : IExchangeClient.OrderSide.Sell;

        public IExchangeClient.OrderType CommonType => Type switch
        {
            KunaOrderType.Limit => IExchangeClient.OrderType.Limit,
            KunaOrderType.Market => IExchangeClient.OrderType.Market,
            KunaOrderType.MarketByQuote => IExchangeClient.OrderType.Market,
            _ => IExchangeClient.OrderType.Other
        };

        public DateTime CommonOrderTime => TimestampCreated;
    }
}