using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kuna.Net
{
    public class KunaSymbolOrderBookOptions : OrderBookOptions
    {
        public readonly int? EntriesCount;
        public readonly int? UpdateTimeout;

        public KunaSymbolOrderBookOptions(string name, int? limit=null, int? timeout=300) : base(name, false)
        {
            EntriesCount = limit;
            UpdateTimeout = timeout;
           // LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug;            
        }
    }
}
