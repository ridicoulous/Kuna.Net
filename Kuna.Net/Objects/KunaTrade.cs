using Kuna.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Kuna.Net.Objects
{
    public class KunaTrade
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
        public OrderType TradeType { get; set; }
    }

}
