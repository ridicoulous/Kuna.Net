using System.Net;
using CryptoExchange.Net.Objects;

namespace Kuna.Net
{
    public class KunaClientOptions : BaseRestClientOptions 
    {
        
        public bool IsProAccount { get; set; }
        public bool UseSingleApiKey { get; private set; }

        public new KunaApiCredentials ApiCredentials
        {
            set
            {
                base.ApiCredentials = value;
                UseSingleApiKey = value?.UseSingleApiKey ?? false;
            }
        }
        public KunaClientOptions(bool isPro = false)
        {
            IsProAccount = isPro;
        }
    }
}
