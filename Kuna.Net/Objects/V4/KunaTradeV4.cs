using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaUserTradeV4 : KunaPublicTradeV4
    {

        /// <summary>
        /// Unique identifier of an associated order
        /// </summary>
        [JsonProperty("orderId")]
        public Guid OrderId { get; set; }

 
        /// <summary>
        /// Various fees for Makers and Takers; "Market" orders are always `true`
        /// </summary>
        [JsonProperty("isTaker")]
        public bool IsTaker { get; set; }
        /// <summary>
        /// Exchange commission fee
        /// </summary>
        [JsonProperty("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Currency of the commission
        /// </summary>
        [JsonProperty("feeCurrency")]
        public string FeeCurrency { get; set; }
        /// <summary>
        /// Buy or sell the base asset
        /// </summary>
        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }
    }
}