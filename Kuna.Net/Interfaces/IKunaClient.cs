using CryptoExchange.Net.Objects;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient
    {
        CallResult<DateTime> GetServerTime();
        CallResult<KunaTickerInfo> GetMarketInfo(string market);
        CallResult<KunaOrderBook> GetOrderBook(string market);
        CallResult<List<KunaTrade>> GetTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit=100);
        CallResult<KunaAccountInfo> GetAccountInfo();
        CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market);
        CallResult<KunaPlacedOrder> CancelOrder(long orderId);
        CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState=OrderState.Wait, int page = 1, string sort = "desc");
        CallResult<KunaPlacedOrder> GetOrderInfo(long orderId);

        CallResult<List<KunaTrade>> GetMyTrades(string market);
        CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo();
    }
}
