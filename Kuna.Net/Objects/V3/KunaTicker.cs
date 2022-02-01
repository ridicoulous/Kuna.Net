using CryptoExchange.Net.Converters;

using Newtonsoft.Json;

namespace Kuna.Net.Objects.V3
{
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaTicker
    {
        [ArrayProperty(0)]
        public string Symbol { get; set; }
       
        [ArrayProperty(1)]
        public decimal Bid { get; set; }
        /// <summary>
        /// Sum of bid sizes
        /// </summary>
        [ArrayProperty(2)]
        public decimal LiquidityBids { get; set; }

        [ArrayProperty(3)]
        public decimal Ask { get; set; }
        /// <summary>
        /// Sum of ask sizes
        /// </summary>
        [ArrayProperty(4)]
        public decimal LiquidityAsks { get; set; }
        /// <summary>
        /// Amount that the last price has changed since yesterday
        /// </summary>
        [ArrayProperty(5)]
        public decimal PriceDiffQuoteCurr { get; set; }
        /// <summary>
        /// Amount that the price has changed expressed in percentage terms
        /// </summary>
        [ArrayProperty(6)]
        public decimal PriceDiffPercent { get; set; }

        [ArrayProperty(7)]
        public decimal LastPrice { get; set; }

        [ArrayProperty(8)]
        public decimal Volume { get; set; }

        [ArrayProperty(9)]
        public decimal High { get; set; }

        [ArrayProperty(10)]
        public decimal Low { get; set; }

        public string CommonSymbol => Symbol;

        public decimal CommonHigh => High;

        public decimal CommonLow => Low;

        public decimal CommonVolume => Volume;
    }
}