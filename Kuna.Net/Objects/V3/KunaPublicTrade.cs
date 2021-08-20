using System;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V3
{
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaPublicTrade: ICommonRecentTrade
    {
        [ArrayProperty(0)]
        public long Id { get; set; }

        /// <summary>
        /// The time the trade was executed
        /// </summary>
        [ArrayProperty(1), JsonConverter(typeof(TimestampConverter))]
        public DateTime TradeTime { get; set; }

        [ArrayProperty(2)]
        public decimal Quantity { get; set; }

        [ArrayProperty(3)]
        public decimal Price { get; set; }

        public KunaOrderSide Side => Quantity > 0? KunaOrderSide.Buy : KunaOrderSide.Sell;

        public decimal CommonPrice => Price;

        public decimal CommonQuantity => Math.Abs(Quantity);

        public DateTime CommonTradeTime => TradeTime;
    }
}