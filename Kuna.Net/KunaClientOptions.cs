using CryptoExchange.Net.Objects;

namespace Kuna.Net
{
    public class KunaClientOptions : RestClientOptions
    {
        public KunaClientOptions(string baseAddress= "https://kuna.io/api/v2") :base(baseAddress)
        {            

        }
    }
}
