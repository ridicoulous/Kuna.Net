using CryptoExchange.Net.Objects;
using System;

namespace Kuna.Net
{
    public class KunaApiClientOptions : RestApiClientOptions
    {
        // public bool IsProAccount { get; set; }
        public KunaApiClientOptions() : base("https://api.kuna.io/")
        {
        }
    }
}
