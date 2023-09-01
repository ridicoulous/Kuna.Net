using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net.Converters;

using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaOrderBookV4 
    {
        [JsonProperty("asks")]
        public IEnumerable<KunaOrderBookEntryV4> Asks { get; set; }

        [JsonProperty("bids")]
        public IEnumerable<KunaOrderBookEntryV4> Bids { get; set; }

    }

    [JsonConverter(typeof(ArrayConverter))]
    public class KunaOrderBookEntryV4 : ISymbolOrderBookEntry
    {
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}