using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kuna.Net
{
    public class KunaSymbolOrderBookOptions : OrderBookOptions
    {
        public readonly int EntriesCount;
        public readonly int UpdateTimeout;
        public readonly int ResponseTimeout;
    

        public KunaSymbolOrderBookOptions(string name, int limit = 100, int? timeout = 300, int? responseTimeout = 3) : base(name, false)
        {
            EntriesCount = limit;
           
           // LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug;            
            UpdateTimeout = timeout ?? 300;
            ResponseTimeout = responseTimeout ?? 3;
            // LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug;            
        }
    }
}
