using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaSocketTrade : KunaPublicTrade
    {
         [JsonProperty("type")]
        public KunaOrderSide Type { set => Side = value; }

    }
}