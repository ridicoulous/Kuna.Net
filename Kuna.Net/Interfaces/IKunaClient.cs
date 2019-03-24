using CryptoExchange.Net.Objects;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);
        CallResult<DateTime> GetServerTime();
        CallResult<KunaTickerInfo> GetMarketInfo(string market);
        CallResult<KunaOrderBook> GetOrderBook(string market);
        CallResult<List<KunaTrade>> GetTrades(string market);
        CallResult<KunaAccountInfo> GetAccountInfo();
        CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market);
        CallResult<KunaPlacedOrder> CancelOrder(long orderId);
        CallResult<List<KunaPlacedOrder>> GetActiveOrders(string market);
        CallResult<List<KunaTrade>> GetMyTrades(string market);
    }
}
