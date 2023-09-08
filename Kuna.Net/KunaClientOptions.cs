using CryptoExchange.Net.Objects.Options;

namespace Kuna.Net
{
    public class KunaRestOptions : RestExchangeOptions 
    {
        
        public bool IsProAccount { get; set; }
        internal bool UseSingleApiKey { get; private set; }

        public new KunaApiCredentials ApiCredentials
        {
            set
            {
                base.ApiCredentials = value;
                UseSingleApiKey = value?.UseSingleApiKey ?? false;
            }
        }
        public KunaRestOptions(bool isPro)
        {
            IsProAccount = isPro;
        }

        public KunaRestOptions()
        {
        }
    }
}
