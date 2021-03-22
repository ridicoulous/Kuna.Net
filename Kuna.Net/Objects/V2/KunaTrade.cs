using Kuna.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Kuna.Net.Objects.V2
{
    public class KunaTradeV2
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("price"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }

        [JsonProperty("volume"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Volume { get; set; }

        [JsonProperty("funds"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal TradeSumm { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("created_at"), JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("trend"), JsonConverter(typeof(OrderSideConverter))]
        public KunaOrderSideV2 TradeType { get; set; }
        public bool IsMaker()
        {
            if (String.IsNullOrEmpty(Side))
            {
                return false;
            }
            if (Side == "ask")
            {
                return TradeType == KunaOrderSideV2.Buy;
            }
            if (Side == "bid")
            {
                return TradeType == KunaOrderSideV2.Sell;
            }
            else return false;

        }

    }


}
