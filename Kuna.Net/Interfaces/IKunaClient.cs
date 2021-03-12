using CryptoExchange.Net.Objects;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient
    {
        CallResult<DateTime> GetServerTime();
        Task<CallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        CallResult<KunaTickerInfo> GetMarketInfo(string market);
        Task<CallResult<KunaTickerInfo>> GetMarketInfoAsync(string market, CancellationToken ct = default);

        CallResult<KunaOrderBook> GetOrderBook(string market, int limit = 1000);
        Task<CallResult<KunaOrderBook>> GetOrderBookAsync(string market, int limit = 1000, CancellationToken ct = default);

        CallResult<List<KunaTrade>> GetTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");
        Task<CallResult<List<KunaTrade>>> GetTradesAsync(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc", CancellationToken ct = default);

        CallResult<KunaAccountInfo> GetAccountInfo();
        Task<CallResult<KunaAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default);

        CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market);
        Task<CallResult<KunaPlacedOrder>> PlaceOrderAsync(OrderType type, OrderSide side, decimal volume, decimal price, string market, CancellationToken ct = default);

        CallResult<KunaPlacedOrder> CancelOrder(long orderId);
        Task<CallResult<KunaPlacedOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default);

        CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc");
        Task<CallResult<List<KunaPlacedOrder>>> GetMyOrdersAsync(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc", CancellationToken ct = default);

        CallResult<KunaPlacedOrder> GetOrderInfo(long orderId);
        Task<CallResult<KunaPlacedOrder>> GetOrderInfoAsync(long orderId, CancellationToken ct = default);


        CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");
        Task<CallResult<List<KunaTrade>>> GetMyTradesAsync(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc", CancellationToken ct = default);

        CallResult<List<KunaTraidingPair>> GeMarkets();

        Task<CallResult<List<KunaTraidingPair>>> GeMarketsAsync(CancellationToken ct = default);
        CallResult<List<KunaOhclv>> GetCandlesHistory(string symbol, int resolution, DateTime from, DateTime to);

        Task<CallResult<List<KunaOhclv>>> GetCandlesHistoryAsync(string symbol, int resolution, DateTime from, DateTime to, CancellationToken token = default);
        Task<CallResult<List<KunaPlacedOrderV3>>> GetOrders3Async(OrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);
        CallResult<List<KunaPlacedOrderV3>> GetOrders3(OrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null);
        CallResult<List<KunaTrade3>> GetOrderTrades(string market, long id);
        Task<CallResult<List<KunaTrade3>>> GetOrderTradesAsync(string market, long id, CancellationToken ct);
        CallResult<List<KunaCurrency>> GetCurrencies(CancellationToken ct = default);
        Task<CallResult<List<KunaCurrency>>> GetCurrenciesAsync(CancellationToken ct = default);



    }
}
