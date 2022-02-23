using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using System.Collections.Generic;

namespace Kuna.Net.Converters
{
    public class OrderStatusV2Converter : BaseConverter<KunaOrderStateV2>
    {
        public OrderStatusV2Converter() : this(false) { }
        public OrderStatusV2Converter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderStateV2, string>> Mapping => new List<KeyValuePair<KunaOrderStateV2, string>>
        {
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Cancel, "cancel"),
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Done, "done"),
            new KeyValuePair<KunaOrderStateV2, string>(KunaOrderStateV2.Wait, "wait")


        };
    }
}
