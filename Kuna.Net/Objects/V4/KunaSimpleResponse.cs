using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaSimpleResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
    
    public class KunaCancelBulkOrderResponse : KunaSimpleResponse
    {
        [JsonProperty("orderId")]
        public Guid OrdId { get; set; }
    }

}