using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using System.Collections.Generic;

namespace Kuna.Net.Converters
{
    public class OrderTypeV2Converter : BaseConverter<KunaOrderTypeV2>
    {
        public OrderTypeV2Converter() : this(false) { }
        public OrderTypeV2Converter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderTypeV2, string>> Mapping => new List<KeyValuePair<KunaOrderTypeV2, string>>
        {
            new KeyValuePair<KunaOrderTypeV2, string>(KunaOrderTypeV2.Limit, "limit"),
            new KeyValuePair<KunaOrderTypeV2, string>(KunaOrderTypeV2.Market, "market")

        };
    }
}
