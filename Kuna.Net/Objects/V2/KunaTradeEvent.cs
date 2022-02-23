using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kuna.Net.Objects.V2
{
    public  class KunaTradeEventV2
    {
        [JsonProperty("trades")]
        public List<TradeV2> Trades { get; set; }
    }

    public class TradeV2
    {
        [JsonProperty("tid")]
        public long Tid { get; set; }

        [JsonProperty("type"), JsonConverter(typeof(OrderSideConverter))]
        public KunaOrderSideV2 Type { get; set; }

        [JsonProperty("date"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("price"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }

        [JsonProperty("amount"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Amount { get; set; }
    }
}
