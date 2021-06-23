using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V3;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Converters
{
    public class KunaOrderStatusConverter : BaseConverter<KunaOrderStatus>
    {
        public KunaOrderStatusConverter() : this(false) { }
        public KunaOrderStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderStatus, string>> Mapping => new List<KeyValuePair<KunaOrderStatus, string>>
        {
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Canceled, "CANCELED"),
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Canceled, "Cancel"),
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Active, "Wait"),
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Filled, "Done"),

            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Filled, "EXECUTED"),
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Active, "ACTIVE"),
            new KeyValuePair<KunaOrderStatus, string>(KunaOrderStatus.Undefined, "*")
        };
    }
}
