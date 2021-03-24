using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Kuna.Net.Objects.V3;

namespace Kuna.Net.Converters
{
    public class OrderTypeConverter: BaseConverter<KunaOrderType>
    {
        public OrderTypeConverter() : this(false) { }
        public OrderTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KunaOrderType, string>> Mapping => new List<KeyValuePair<KunaOrderType, string>>
        {
            new KeyValuePair<KunaOrderType, string>(KunaOrderType.Limit, "limit"),
            new KeyValuePair<KunaOrderType, string>(KunaOrderType.Market, "market"),
            new KeyValuePair<KunaOrderType, string>(KunaOrderType.StopLimit, "limit_stop_loss"),
            new KeyValuePair<KunaOrderType, string>(KunaOrderType.MarketByQuote, "market_by_quote")

        };
    }
}