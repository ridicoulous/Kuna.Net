using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Converters
{
    public class OrderTypeConverter : BaseConverter<KunaOrderTypeV2>
    {
        public OrderTypeConverter() : this(false) { }
        public OrderTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderTypeV2, string>> Mapping => new List<KeyValuePair<KunaOrderTypeV2, string>>
        {
            new KeyValuePair<KunaOrderTypeV2, string>(KunaOrderTypeV2.Limit, "limit"),
            new KeyValuePair<KunaOrderTypeV2, string>(KunaOrderTypeV2.Market, "market")

        };
    }
}
