using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects.V2
{
    public enum KunaOrderSideV2
    {
        Buy,
        Sell
    }
    public enum KunaOrderTypeV2
    {
        Limit,
        Market
    }
    public enum KunaOrderStateV2
    {
        Wait,
        Cancel,
        Done
    }
}
