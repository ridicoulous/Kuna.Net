using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Helpers
{
    internal class OrderBookEntryComparer : IEqualityComparer<ISymbolOrderBookEntry>
    {
        public bool Equals(ISymbolOrderBookEntry x, ISymbolOrderBookEntry y)
        {
            return x.Price == y.Price&x.Quantity==y.Quantity;
        }

        public int GetHashCode(ISymbolOrderBookEntry obj)
        {
            return obj.Price.GetHashCode() ^ obj.Quantity.GetHashCode();
        }
    }
}
