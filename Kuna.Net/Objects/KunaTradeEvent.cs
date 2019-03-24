using CryptoExchange.Net.Converters;
using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public  class KunaTradeEvent
    {
        [JsonProperty("trades")]
        public List<Trade> Trades { get; set; }
    }

    public class Trade
    {
        [JsonProperty("tid")]
        public long Tid { get; set; }

        [JsonProperty("type"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Type { get; set; }

        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("price"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }

        [JsonProperty("amount"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Amount { get; set; }
    }
}
