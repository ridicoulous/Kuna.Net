using CryptoExchange.Net.Objects;
using System;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {
        public KunaClientOptions(string baseAddress= "https://kuna.io/api/v2") :base(baseAddress)
        {
            this.RequestTimeout = TimeSpan.FromMilliseconds(1500);
        }
    }
}
