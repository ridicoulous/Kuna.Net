using CryptoExchange.Net.Interfaces;
using System.Collections.Generic;

namespace Kuna.Net.Converters
{
    public class SymbolOrderBookEntryEqualityComparer : IEqualityComparer<ISymbolOrderBookEntry>
    {
        public bool Equals(ISymbolOrderBookEntry x, ISymbolOrderBookEntry y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            return x.Price == y.Price && x.Quantity == y.Quantity;
        }

        public int GetHashCode(ISymbolOrderBookEntry obj)
        {
            return obj.Price.GetHashCode() ^ obj.Quantity.GetHashCode()^obj.GetHashCode();
        }
    }
}
