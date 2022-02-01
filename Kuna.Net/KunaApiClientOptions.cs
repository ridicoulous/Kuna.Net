using CryptoExchange.Net.Objects;
using System;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaApiClientOptions : RestApiClientOptions
    {
        public bool IsProAccount { get; set; }
        public KunaApiClientOptions(bool isPro = false, bool newVersion = true) : base(newVersion ? "https://api.kuna.io/v3/" : "https://kuna.io/api/v2/")
        {
            IsProAccount = isPro;
            if (newVersion)
            {
                RateLimiters.Add(new RateLimiter().AddTotalRateLimit(isPro ? 1200 : 600, TimeSpan.FromMinutes(1)));
            }
        }
    }
}
