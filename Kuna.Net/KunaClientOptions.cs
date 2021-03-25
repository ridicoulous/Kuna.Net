using CryptoExchange.Net.Objects;
using System;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {
        public bool IsProAccount { get; set; }
        public KunaClientOptions(HttpClient httpClient, bool isPro = false) :base(httpClient,"https://kuna.io/api/v2")
        {
            IsProAccount = isPro;
        }
        public KunaClientOptions(bool isPro=false, string baseAddress= "https://kuna.io/api/v2", TimeSpan? timeout=null) :base(baseAddress)
        {
            IsProAccount = isPro;
            this.RequestTimeout = timeout?? TimeSpan.FromMilliseconds(3000);
        }
    }
}
