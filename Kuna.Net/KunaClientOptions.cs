using CryptoExchange.Net.Objects;
using System;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {

        public KunaClientOptions(HttpClient httpClient):base("https://kuna.io/api/v2")
        {

        }
        public KunaClientOptions(string baseAddress= "https://kuna.io/api/v2", TimeSpan? timeout=null) :base(baseAddress)
        {
            this.RequestTimeout = timeout?? TimeSpan.FromMilliseconds(3000);
        }
    }
}
