using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Converters
{
    public class OrderSideConverter : BaseConverter<KunaOrderSideV2>
    {
        public OrderSideConverter() : this(false) { }
        public OrderSideConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderSideV2, string>> Mapping => new List<KeyValuePair<KunaOrderSideV2, string>>
        {
            new KeyValuePair<KunaOrderSideV2, string>(KunaOrderSideV2.Buy, "buy"),
            new KeyValuePair<KunaOrderSideV2, string>(KunaOrderSideV2.Sell, "sell")

        };
    }
}
