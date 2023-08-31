using System;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4
{
    public class KunaCurrencyV4
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("precision")]
        public int Precision { get; set; }

        [JsonProperty("tradePrecision")]
        public int TradePrecision { get; set; }

        [JsonProperty("type")]
        public CurrencyType Type { get; set; }
    }

    public class Payload
    {
        [JsonProperty("chart")]
        public Uri Chart { get; set; }

        [JsonProperty("icons")]
         public Icons Icons { get; set; }

        [JsonProperty("pngChart")]
        public Uri PngChart { get; set; }
    }

    public class Icons
    {
        [JsonProperty("svg")]
        public Uri Svg { get; set; }

        [JsonProperty("png2x")]
        public Uri Png2X { get; set; }

        [JsonProperty("png3x")]
        public Uri Png3X { get; set; }

        [JsonProperty("svgXL")]
        public Uri SvgXl { get; set; }
    }
}