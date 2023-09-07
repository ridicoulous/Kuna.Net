using System.Runtime.Serialization;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CurrencyType
    {
        Fiat,
        Crypto
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KunaOrderTypeV4
    {
        Limit,
        Market,
        StopLossLimit,
        TakeProfitLimit
    }

    // [JsonConverter(typeof(OrderSideConverter))]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KunaOrderSideV4
    {
        /// <summary>
        /// for buying base asset
        /// </summary>
        [EnumMember(Value = "Bid")]
        Buy,
        /// <summary>
        /// for selling base asset
        /// </summary>
        [EnumMember(Value = "Ask")]
        Sell
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

