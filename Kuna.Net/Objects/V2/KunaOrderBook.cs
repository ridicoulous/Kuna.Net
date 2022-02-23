using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuna.Net.Objects.V2
{
    public class KunaOrderBookV2
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("asks")]
        public List<KunaOrderBookEntryV2> Asks { get; set; }
        [JsonProperty("bids")]
        public List<KunaOrderBookEntryV2> Bids { get; set; }
    }
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaOrderBookEntryV2 : ISymbolOrderBookEntry
    {
        public KunaOrderBookEntryV2()
        {

        }
        public KunaOrderBookEntryV2(decimal price, decimal amount, int count=1)
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
