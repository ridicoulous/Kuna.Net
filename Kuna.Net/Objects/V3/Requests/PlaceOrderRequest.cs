using Newtonsoft.Json;

namespace Kuna.Net.Objects.V3.Requests
{
    public class PlaceOrderRequest
    {
        private KunaOrderSide? side;

        public PlaceOrderRequest()
        {
        }

        public PlaceOrderRequest(string symbol, KunaOrderSide side, KunaOrderType type, decimal quantity, decimal? price = null, decimal? stopPrice = null): this(symbol, type, quantity, price, stopPrice)
        {
            Side = side;
        }

        public PlaceOrderRequest(string symbol, KunaOrderType type, decimal quantity,decimal? price, decimal? stopPrice)
        {
            Symbol = symbol;
            Type = type;
            Quantity = quantity;
            StopPrice = stopPrice;
            Price = price;
        }

        /// <summary>
        /// Trading pair. Case insensitive
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// limit, market, market_by_quote, limit_stop_loss
        /// </summary>
        [JsonProperty("type")]
        public KunaOrderType Type { get; set; }

        /// <summary>
        /// negative value for sell, positive for buy
        /// </summary>
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// for stop-limit, if omitted used price
        /// </summary>
        [JsonProperty("stop_price")]
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Order price. Required for limit order types
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonIgnore]
        private KunaOrderSide? Side
        {
            get => side;
            set
            {
                this.side = value;
                if ((side == KunaOrderSide.Sell && Quantity > 0)
                    || (side == KunaOrderSide.Buy && Quantity < 0))
                {
                    Quantity *= -1;
                }
            }
        }
    }
}