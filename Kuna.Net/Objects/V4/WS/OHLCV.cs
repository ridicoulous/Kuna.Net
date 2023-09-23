using System;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaOHLCVBase
    {
        public DateTimeOffset OpenTime { get; set; } = DateTimeOffset.UtcNow.AddDays(-1);

        public DateTimeOffset CloseTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// price of the trade happenned after OpenTime
        /// </summary>
        [JsonProperty("o")]
        public virtual decimal Open { get; set; }

        [JsonProperty("h")]
        public decimal High { get; set; }

        [JsonProperty("l")]
        public decimal Low { get; set; }

        [JsonProperty("c")]
        public decimal Close { get; set; }

        [JsonProperty("v")]
        public decimal Volume { get; set; }
    }

    public class KunaOHLCV : KunaOHLCVBase
    {
        [JsonProperty("ot"), JsonConverter(typeof(DateTimeConverter))]
        public DateTimeOffset OT { set => OpenTime = value; }

        [JsonProperty("ct"), JsonConverter(typeof(DateTimeConverter))]
        public DateTimeOffset CT { set => CloseTime = value; }

        [JsonProperty("t")]
        public int TradesAmount { get; set; }
    }
}