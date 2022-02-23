using Kuna.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kuna.Net.Objects.V2
{
    public class KunaAccountInfoV2
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("activated")]
        public bool Activated { get; set; }

        [JsonProperty("accounts")]
        public List<AccountV2> Accounts { get; set; }
    }

    public class AccountV2
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("balance"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal FreeBalance { get; set; }

        [JsonProperty("locked"), JsonConverter(typeof(StringToDecimalConverter))]
        public decimal LockedBalance { get; set; }
        public decimal TotalBalance => FreeBalance + LockedBalance;
    }

}
