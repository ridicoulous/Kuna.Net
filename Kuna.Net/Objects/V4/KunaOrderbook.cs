using System.Collections.Generic;
using CryptoExchange.Net.Converters;

using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaOrderBook 
    {
        [JsonProperty("asks")]
        public IEnumerable<KunaOrderBookEntry> Asks { get; set; }

        [JsonProperty("bids")]
        public IEnumerable<KunaOrderBookEntry> Bids { get; set; }

    }

    [JsonConverter(typeof(ArrayConverter))]
    public class KunaOrderBookEntry : ISymbolOrderBookEntry
    {
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }

    public class KunaSocketUpdateOrderBook : KunaOrderBook
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

    }
}