using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class KunaSocketTrade : KunaTradeV4
    {
         [JsonProperty("type")]
        public KunaOrderSideV4 Type { set => Side = value; }

    }
}