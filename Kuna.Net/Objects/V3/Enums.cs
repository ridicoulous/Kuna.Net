using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects.V3
{
    public enum KunaOrderSide
    {
        Buy,
        Sell
    }
    public enum KunaOrderType
    {
        Limit,
        Market,
        MarketByQuote,
        StopLimit
    }
    public enum KunaOrderState
    {
        Wait,
        Cancel,
        Done
    }
}
