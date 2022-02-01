using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V3
{
    [JsonConverter(typeof(ArrayConverter))]
    public class KunaAccountBalance
    {
        [ArrayProperty(0)]
        public string WalletType { get; set; }

        [ArrayProperty(1)]
        public string Currency { get; set; }
        [ArrayProperty(2)]
        public decimal Total { get; set; }
        [ArrayProperty(4)]
        public decimal Available { get; set; }

        public string CommonAsset => Currency;

        public decimal CommonAvailable => Available;

        public decimal CommonTotal => Total;
    }
}