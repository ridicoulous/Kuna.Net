using CryptoExchange.Net.Objects;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient
    {
        CallResult<DateTime> GetServerTime();
        Task<CallResult<DateTime>> GetServerTimeAsync();

        CallResult<KunaTickerInfo> GetMarketInfo(string market);
        Task<CallResult<KunaTickerInfo>> GetMarketInfoAsync(string market);

        CallResult<KunaOrderBook> GetOrderBook(string market);
        Task<CallResult<KunaOrderBook>> GetOrderBookAsync(string market);

        CallResult<List<KunaTrade>> GetTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit=1000, string sort = "asc");
        Task<CallResult<List<KunaTrade>>> GetTradesAsync(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");

        CallResult<KunaAccountInfo> GetAccountInfo();
        Task<CallResult<KunaAccountInfo>> GetAccountInfoAsync();
       
        CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market);
        Task<CallResult<KunaPlacedOrder>> PlaceOrderAsync(OrderType type, OrderSide side, decimal volume, decimal price, string market);

        CallResult<KunaPlacedOrder> CancelOrder(long orderId);
        Task<CallResult<KunaPlacedOrder>> CancelOrderAsync(long orderId);

        CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState=OrderState.Wait, int page = 1, string sort = "desc");
        Task<CallResult<List<KunaPlacedOrder>>> GetMyOrdersAsync(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc");

        CallResult<KunaPlacedOrder> GetOrderInfo(long orderId);
        Task<CallResult<KunaPlacedOrder>> GetOrderInfoAsync(long orderId);


        CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort="asc");
        Task<CallResult<List<KunaTrade>>> GetMyTradesAsync(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");

        CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo();
        Task<CallResult<List<KunaTraidingPair>>> GetExchangeCurrenciesInfoAsync();

    }
}
