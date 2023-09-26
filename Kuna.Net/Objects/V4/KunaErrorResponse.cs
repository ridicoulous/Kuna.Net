using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaErrorResponse
    {
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        [JsonProperty("extensions")]
        public Extensions Extensions { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Extensions
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}