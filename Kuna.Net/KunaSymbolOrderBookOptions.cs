using CryptoExchange.Net.Objects;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaSymbolOrderBookOptions : OrderBookOptions
    {
        public readonly int EntriesCount;
        public readonly int UpdateTimeout;
        public readonly int ResponseTimeout;
        public readonly bool Usev3;
        public readonly HttpClient HttpClient;
        public KunaSymbolOrderBookOptions(string name, int limit = 100, int? timeout = 300, int? responseTimeout = 3, bool v3=false) //: base(name, false,false)
        {
            EntriesCount = limit;
            Usev3 = v3;
            UpdateTimeout = timeout ?? 300;
            ResponseTimeout = responseTimeout ?? 3;            
        }
    }
}
