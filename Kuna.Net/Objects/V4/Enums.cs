namespace Kuna.Net.Objects.V4
{
    // 5, 10, 20, 50, 100, 500, 1000
    public enum OrderBookLevel
    {
        Five = 5,
        Ten = 10,
        Twenty = 20,
        Fifty = 50,
        OneHundred = 100,
        FiveHundred = 500,
        OneThousand = 1000,
    }
    public enum CurrencyType
    {
        Fiat,
        Crypto
    }
    public enum KunaOrderTypeV4
    {
        Limit,
        Market,
        StopLossLimit,
        TakeProfitLimit
    }

    public enum KunaOrderSideV4
    {
        /// <summary>
        /// for buying base asset
        /// </summary>
        Bid,
        /// <summary>
        /// for selling base asset
        /// </summary>
        Ask
    }
    public enum KunaOrderStatusV4
    {
        Canceled, 
        Closed, 
        Expired, 
        Open, 
        Pending, 
        Rejected, 
        WaitStop
    }
}

