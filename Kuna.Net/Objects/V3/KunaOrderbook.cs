using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;

namespace Kuna.Net.Objects.V3
{
    public class KunaOrderBook
    {
        public KunaOrderBook(List<KunaOrderBookEntry> asks, List<KunaOrderBookEntry> bids)
        {
            Asks = asks;
            Bids = bids;
        }
        public KunaOrderBook(IEnumerable<KunaOrderBookEntry> entries)
        {
            Asks = new List<KunaOrderBookEntry>(entries.Where(e => e.Quantity < 0));
            Bids = new List<KunaOrderBookEntry>(entries.Where(e => e.Quantity > 0));
        }
        public List<KunaOrderBookEntry> Asks { get; set; }
        public List<KunaOrderBookEntry> Bids { get; set; }
    }


    public class KunaOrderBookEntry : ISymbolOrderBookEntry
    {
        public KunaOrderBookEntry()
        {

        }
        public KunaOrderBookEntry(decimal price, decimal amount, int count = 1)
        {
            Price = price;
            Quantity = amount;
            Count = count;
        }
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
        [ArrayProperty(2)]
        public int Count { get; set; }
    }
}