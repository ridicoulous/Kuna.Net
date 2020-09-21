using Kuna.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public class KunaAccountInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("activated")]
        public bool Activated { get; set; }

        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }
    }

    public class Account
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
