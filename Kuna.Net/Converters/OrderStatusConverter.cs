using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Converters
{
    public class OrderStatusConverter : BaseConverter<KunaOrderStateV2>
    {
        public OrderStatusConverter() : this(false) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderStateV2, string>> Mapping => new List<KeyValuePair<KunaOrderStateV2, string>>
        {
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Cancel, "cancel"),
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Done, "done"),
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Wait, "wait")


        };
    }
}
