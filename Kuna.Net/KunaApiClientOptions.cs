using CryptoExchange.Net.Objects;
using System;

namespace Kuna.Net
{
    public class KunaApiClientOptions : RestApiClientOptions
    {
        public bool IsProAccount { get; set; }
        public KunaApiClientOptions(bool isPro = false, bool newVersion = true) : base(newVersion ?  "https://api.kuna.io/" : "https://api.kuna.io/v3/")
        {
            IsProAccount = isPro;
            RateLimiters.Add(new RateLimiter().AddTotalRateLimit(isPro ? 1200 : 600, TimeSpan.FromMinutes(1)));
        }
    }
}
