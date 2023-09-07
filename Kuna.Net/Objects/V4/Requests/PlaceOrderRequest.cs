using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.Requests
{
    public class PlaceOrderRequestV4
    {

        private PlaceOrderRequestV4(string symbol, KunaOrderSideV4 orderSide, Guid? id)
        {
            Symbol = symbol;
            Side = orderSide;
            Id = id;
        }

        /// <summary>
        /// id must be a UUID format. If you do not specify id, it will be generated automatically.
        /// </summary>
        [JsonProperty("id")]
        public Guid? Id { get; private set;}

        /// <summary>
        /// Trading pair.
        /// </summary>
        [JsonProperty("pair")]
        public string Symbol { get; private set;}

        /// <summary>
        /// limit, market, market_by_quote, limit_stop_loss
        /// </summary>
        [JsonProperty("type")]
        public KunaOrderTypeV4 Type { get; private set;}

        /// <summary>
        /// Quantity of the base asset to buy/sell (e.g. for BTC_USDT trading pair, BTC is the base asset).
        /// Precision depends on the asset as per Get all traded markets endpoint.
        /// </summary>
        [JsonProperty("quantity")]
        public string Quantity { get; private set;}

        /// <summary>
        /// The max quantity of the quote asset to use for selling/buying (e.g. for BTC_USDT trading pair, USDT is the quote asset).
        /// This field is only available for Market orders.
        /// Precision depends on the asset as per Get all traded markets endpoint.
        /// </summary>
        [JsonProperty("quoteQuantity")]
        public string QuoteQuantity { get; private set;}

        /// <summary>
        /// It is a trigger for the limit price: 
        /// when the coin's market price reaches stopPrice, a limit order is automatically placed.
        /// </summary>
        [JsonProperty("stopPrice")]
        public string StopPrice { get; private set;}

        /// <summary>
        /// Order price. Required for limit order types
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; private set;}

        [JsonProperty("orderSide")]
        public KunaOrderSideV4 Side { get; private set; }

        public static PlaceOrderRequestV4 LimitOrder(string symbol, decimal amount, KunaOrderSideV4 orderSide, decimal price, Guid? id = null)
        {
            return new PlaceOrderRequestV4(symbol, orderSide, id)
            {
                Type = KunaOrderTypeV4.Limit,
                Quantity = amount.ToString(CultureInfo.InvariantCulture),
                Price = price.ToString(CultureInfo.InvariantCulture),
            };
        }
        /// <summary>
        /// Create request for market order type. set isAmountPerQuoteAsset=true if you want to set amount of quote asset, not base
        /// (e.g. for BTC_USDT trading pair, USDT is the quote asset).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="amount"></param>
        /// <param name="orderSide"></param>
        /// <param name="isAmountPerQuoteAsset"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PlaceOrderRequestV4 MarketOrder(string symbol, decimal amount, KunaOrderSideV4 orderSide, bool isAmountPerQuoteAsset = false, Guid? id = null)
        {
            var req =  new PlaceOrderRequestV4(symbol, orderSide, id)
            {
                Type = KunaOrderTypeV4.Market,
            };
            if (isAmountPerQuoteAsset)
                req.QuoteQuantity = amount.ToString(CultureInfo.InvariantCulture);
            else
                req.Quantity = amount.ToString(CultureInfo.InvariantCulture);

            return req;
        }
        //TODO: improve with validation and choosing type between StopLossLimit/TakeProfitLimit depending on the price/stopprice
        public static PlaceOrderRequestV4 StopLossLimit(string symbol, decimal amount, KunaOrderSideV4 orderSide, decimal price, decimal triggerPrice, Guid? id = null)
        {
            return new PlaceOrderRequestV4(symbol, orderSide, id)
            {
                Type = KunaOrderTypeV4.StopLossLimit,
                Price = price.ToString(CultureInfo.InvariantCulture),
                StopPrice = triggerPrice.ToString(CultureInfo.InvariantCulture),
                Quantity = amount.ToString(CultureInfo.InvariantCulture),
            };
        }

        public static PlaceOrderRequestV4 TakeProfitLimit(string symbol, decimal amount, KunaOrderSideV4 orderSide, decimal price, decimal triggerPrice, Guid? id = null)
        {
            return new PlaceOrderRequestV4(symbol, orderSide, id)
            {
                Type = KunaOrderTypeV4.TakeProfitLimit,
                Price = price.ToString(CultureInfo.InvariantCulture),
                StopPrice = triggerPrice.ToString(CultureInfo.InvariantCulture),
                Quantity = amount.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}