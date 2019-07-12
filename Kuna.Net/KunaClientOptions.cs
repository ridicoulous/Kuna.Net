using CryptoExchange.Net.Objects;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {
        public KunaClientOptions()
        {
            BaseAddress = "https://kuna.io/api/v2";

        }
    }
}
