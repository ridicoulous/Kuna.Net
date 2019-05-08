using CryptoExchange.Net.Converters;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Converters
{
    public class OrderStatusConverter : BaseConverter<OrderState>
    {
        public OrderStatusConverter() : this(false) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderState, string>> Mapping => new List<KeyValuePair<OrderState, string>>
        {
            new KeyValuePair<OrderState, string>(OrderState.Cancel, "cancel"),
            new KeyValuePair<OrderState, string>(OrderState.Done, "done"),
            new KeyValuePair<OrderState, string>(OrderState.Wait, "wait")


        };
    }
}
