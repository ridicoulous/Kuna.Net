using System.IO;
using System.Security;
using CryptoExchange.Net.Authentication;

namespace Kuna.Net
{
    public class KunaApiCredentials : ApiCredentials
    {
        internal bool UseSingleApiKey { get; private set; }
        public KunaApiCredentials(string singleApiKey) : base("just_placeholder", singleApiKey)
        {
            UseSingleApiKey = true;
        }

        public KunaApiCredentials(SecureString publicKey, SecureString privateKey) : base(publicKey, privateKey)
        {
        }

        public KunaApiCredentials(string publicKey, string privateKey) : base(publicKey, privateKey)
        {
        }
    }
}