using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using Kuna.Net.Objects.V3;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClientV3 : IExchangeClient
    {
        WebCallResult<DateTime?> GetServerTime();
        Task<WebCallResult<DateTime?>> GetServerTimeAsync(CancellationToken ct = default);
        WebCallResult<IEnumerable<KunaTradingPair>> GetTradingPairs();
        Task<WebCallResult<IEnumerable<KunaTradingPair>>> GetTradingPairsAsync(CancellationToken ct = default);
        /// <summary>
        /// Get tickers data
        /// Get https://api.kuna.io:443/v3/tickers
        /// </summary>
        /// <param name="symbols">list of explicit market codes</param>
        /// <returns></returns>
        WebCallResult<IEnumerable<KunaTicker>> GetTickers(params string[] symbols);

        /// <summary>
        /// Get tickers data
        /// Get https://api.kuna.io:443/v3/tickers
        /// </summary>
        /// <param name="symbols">list of explicit market codes</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<KunaTicker>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols);
        Task<WebCallResult<List<KunaPlacedOrder>>> GetOrdersAsync(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);
        WebCallResult<List<KunaPlacedOrder>> GetOrders(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null);

        /// <summary>
        /// Cancel multiple orders
        ///  Post https://api.kuna.io:443/v3/order/cancel
        /// </summary>
        /// <param name="orderId">list of order ids, e.g."order_id": 1000000</param>
        /// <returns></returns>
        Task<WebCallResult<KunaCanceledOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default);
        WebCallResult<KunaCanceledOrder> CancelOrder(long orderId);

        /// <summary>
        /// Cancel multiple orders
        ///  Post https://api.kuna.io:443/v3/order/cancel/multi
        /// </summary>
        /// <param name="orderIds">list of order ids, e.g."order_ids": [1000000, 1000001]</param>
        /// <returns></returns>
        Task<WebCallResult<List<KunaPlacedOrder>>> CancelOrdersAsync(List<long> orderIds, CancellationToken ct = default);
        WebCallResult<List<KunaPlacedOrder>> CancelOrders(List<long> orderIds);
        WebCallResult<List<KunaTrade>> GetOrderTrades(string market, long id);
        Task<WebCallResult<List<KunaTrade>>> GetOrderTradesAsync(string market, long id, CancellationToken ct);
        /// <summary>
        /// GET https://api.kuna.io:443/v3/currencies
        /// </summary>
        /// <param name="privileged"></param>
        /// <returns></returns>
        WebCallResult<List<KunaCurrency>> GetCurrencies(bool? privileged = null);
        Task<WebCallResult<List<KunaCurrency>>> GetCurrenciesAsync(bool? privileged = null, CancellationToken ct = default);
        WebCallResult<KunaOrderBook> GetOrderBook(string symbol);
        Task<WebCallResult<KunaOrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default);
       

        Task<WebCallResult<KunaPlacedOrder>> PlaceOrderAsync(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price=null, decimal? stopPrice=null, CancellationToken ct = default);
        WebCallResult<KunaPlacedOrder> PlaceOrder(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price=null, decimal? stopPrice=null);

        /// <summary>
        /// POST https://api.kuna.io:443/v3/auth/r/orders/details
        /// </summary>
        /// <param name="id"> order id</param>
        /// <returns></returns>
        WebCallResult<KunaPlacedOrder> GetOrder(long id);
        /// <summary>
        /// POST https://api.kuna.io:443/v3/auth/r/orders/details
        /// </summary>
        /// <param name="id"> order id</param>
        /// <returns></returns>
        Task<WebCallResult<KunaPlacedOrder>> GetOrderAsync(long id, CancellationToken ct = default);
        /// <summary>
        /// Get https://api.kuna.io:443/v3/trades/{symbol}/hist
        /// </summary>
        /// <param name="symbol">trading pair</param>
        /// <param name="limit">max entries in response</param>
        /// <returns></returns>
        WebCallResult<IEnumerable<KunaPublicTrade>> GetRecentPublicTrades(string symbol, int limit = 25);
        /// <summary>
        /// Get https://api.kuna.io:443/v3/trades/{symbol}/hist
        /// </summary>
        /// <param name="symbol">trading pair</param>
        /// <param name="limit">max entries in response, default 25, max 500</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<KunaPublicTrade>>> GetRecentPublicTradesAsync(string symbol, int limit = 25, CancellationToken ct = default);

        /// <summary>
        /// POST https://api.kuna.io:443/v3/auth/r/wallets
        /// </summary>
        /// <returns></returns>
        WebCallResult<IEnumerable<KunaAccountBalance>> GetBalances();

        /// <summary>
        /// POST https://api.kuna.io:443/v3/auth/r/wallets
        /// </summary>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default);
    }
 
}