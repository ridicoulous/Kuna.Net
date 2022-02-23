using CryptoExchange.Net.Objects;

namespace Kuna.Net.Objects.V2
{
    public class KunaApiCallErrorV2 : Error
    {
        public KunaApiCallErrorV2(int code, string message) : base(code, message,null)
        {
        }
    }
}
