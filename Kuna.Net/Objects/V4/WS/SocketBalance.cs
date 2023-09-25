using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    public class SocketBalance : IEnumerable
    {
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("assets")]
        public IEnumerable<KunaAccountBalance> Assets { get; set; } = Array.Empty<KunaAccountBalance>();

        public IEnumerator GetEnumerator() => Assets.GetEnumerator();
    }
}