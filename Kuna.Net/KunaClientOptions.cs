using CryptoExchange.Net.Objects;
using System;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaClientOptions : BaseRestClientOptions 
    {
        public bool IsProAccount { get; set; }
        public KunaClientOptions(bool isPro = false)
        {
            IsProAccount = isPro;
        }     
    }
}
