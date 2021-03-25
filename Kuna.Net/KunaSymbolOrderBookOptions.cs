using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Kuna.Net
{
    public class KunaSymbolOrderBookOptions : OrderBookOptions
    {
        public readonly int EntriesCount;
        public readonly int UpdateTimeout;
        public readonly int ResponseTimeout;
        public readonly bool Usev3;
        public readonly HttpClient HttpClient;
        public KunaSymbolOrderBookOptions(string name, int limit = 100, int? timeout = 300, int? responseTimeout = 3, bool v3=false, HttpClient client=null) : base(name, false,false)
        {
            EntriesCount = limit;
            Usev3 = v3;
            UpdateTimeout = timeout ?? 300;
            ResponseTimeout = responseTimeout ?? 3;            
        }
    }
}
