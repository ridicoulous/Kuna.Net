using CryptoExchange.Net.Objects;
using System;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {
        public KunaClientOptions(string baseAddress= "https://kuna.io/api/v2", TimeSpan? timeout=null) :base(baseAddress)
        {
            this.RequestTimeout = timeout?? TimeSpan.FromMilliseconds(2700);
        }
    }
}
