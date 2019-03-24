using CryptoExchange.Net.Objects;

namespace Kuna.Net
{
    public class KunaClientOptions : ClientOptions
    {
        public KunaClientOptions()
        {
            BaseAddress = "https://kuna.io/api/v2";

        }
    }
}
