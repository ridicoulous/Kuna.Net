using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    [JsonObject] //add this to explicite deserialize as object, not array
    public class KunaSocketBalance : IEnumerable
    {
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("assets")]
        public IEnumerable<KunaAccountBalance> Assets { get; set; } = Array.Empty<KunaAccountBalance>();

        public IEnumerator GetEnumerator() => Assets.GetEnumerator();
    }
}