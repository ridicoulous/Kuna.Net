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
    public enum KunaOrderStatus
    {
        Active,
        Canceled,
        Filled,
        Undefined
    }
}
