using CryptoExchange.Net.Objects.Options;

namespace Kuna.Net
{
    public class KunaApiClientOptions : RestApiOptions
    {
        public KunaApiClientOptions() : base()
        {
            // wait for aproving pull request
            // IsRateLimitReplacingAllowed = true;
        }
    }
}
