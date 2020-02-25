using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.OrderBook;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuna.Net.Objects
{
    public class KunaOrderBook
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("asks")]
        public List<KunaOrderBookEntry> Asks { get; set; }
        [JsonProperty("bids")]
        public List<KunaOrderBookEntry> Bids { get; set; }
    }
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaOrderBookEntry : ISymbolOrderBookEntry
    {
        public KunaOrderBookEntry()
        {

        }
        public KunaOrderBookEntry(decimal price, decimal amount, int count=1)
        {
            Price = price;
            Quantity = amount;
            Count = count;
        }
        [ArrayProperty(0), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }
        [ArrayProperty(1), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Quantity { get; set; }
        [ArrayProperty(2), JsonConverter(typeof(StringToDecimalConverter))]
        public int Count { get; set; }
    }

}
