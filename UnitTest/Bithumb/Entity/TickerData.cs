using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnitTest.Bithumb.Entity
{
    public class TickerData
    {
        [JsonProperty(PropertyName = "opening_price")]
        public int OpeningPrice { get; set; }
        [JsonProperty(PropertyName = "closing_price")]
        public int ClosingPrice { get; set; }
        [JsonProperty(PropertyName = "min_price")]
        public int MinPrice { get; set; }
        [JsonProperty(PropertyName = "max_price")]
        public int MaxPrice { get; set; }
        [JsonProperty(PropertyName = "average_price")]
        public float AveragePrice { get; set; }
        [JsonProperty(PropertyName = "units_traded")]
        public float UnitsTraded { get; set; }
        [JsonProperty(PropertyName = "volume_1day")]
        public float Volume1Day { get; set; }
        [JsonProperty(PropertyName = "volume_7day")]
        public float Volume7Day { get; set; }
        [JsonProperty(PropertyName = "buy_price")]
        public int BuyPrice { get; set; }
        [JsonProperty(PropertyName = "sell_price")]
        public int SellPrice { get; set; }
    }
}