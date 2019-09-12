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
        public readonly bool UseSocketClient;

        public KunaSymbolOrderBookOptions(bool useSocketClient,string name, int? limit=null) : base(name, false)
        {
            EntriesCount = limit;
            UseSocketClient = useSocketClient;
           // LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug;            
        }
    }
}
