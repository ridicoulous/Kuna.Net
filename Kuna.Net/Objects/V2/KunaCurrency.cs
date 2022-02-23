using Newtonsoft.Json;
using System;

namespace Kuna.Net.Objects.V2
{
    public  class KunaCurrencyV2
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("has_memo")]
        public bool? HasMemo { get; set; }

        [JsonProperty("icons")]
        public IconsV2 Icons { get; set; }

        [JsonProperty("coin")]
        public bool Coin { get; set; }

        [JsonProperty("explorer_link")]
        public string ExplorerLink { get; set; }

        [JsonProperty("sort_order")]
        public long SortOrder { get; set; }

        [JsonProperty("precision")]
        public PrecisionV2 Precision { get; set; }

        [JsonProperty("privileged")]
        public bool Privileged { get; set; }

        [JsonProperty("fuel")]
        public bool? Fuel { get; set; }
    }

    public  class IconsV2
    {
        [JsonProperty("std")]
        public Uri Std { get; set; }

        [JsonProperty("xl")]
        public Uri Xl { get; set; }

        [JsonProperty("png_2x")]
        public Uri Png2X { get; set; }

        [JsonProperty("png_3x")]
        public Uri Png3X { get; set; }
    }

    public  class PrecisionV2
    {
        [JsonProperty("real")]
        public long Real { get; set; }

        [JsonProperty("trade")]
        public long Trade { get; set; }
    }
}
