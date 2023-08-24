using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V4;

namespace Kuna.Net.Interfaces
{
    public interface IKunaApiClientV4 : ISpotClient
    {
        void SetProAccount(bool isProAccountEnabled);
        WebCallResult<DateTime> GetServerTime();
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);
        WebCallResult<IEnumerable<KunaTradingPair>> GetTradingPairs();
        Task<WebCallResult<IEnumerable<KunaTradingPair>>> GetTradingPairsAsync(CancellationToken ct = default);


        /// <summary>
        /// Get tickers data
        /// Get https://api.kuna.io:443/v3/tickers
        /// </summary>
        /// <param name="symbols">list of explicit market codes</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<KunaTicker>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols);
        Task<WebCallResult<List<KunaPlacedOrder>>> GetOrdersAsync(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel multiple orders
        ///  Post https://api.kuna.io:443/v3/order/cancel
        /// </summary>
        /// <param name="orderId">list of order ids, e.g."order_id": 1000000</param>
        /// <returns></returns>
        Task<WebCallResult<KunaCanceledOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default);
        /// <summary>
        /// Cancel multiple orders
        ///  Post https://api.kuna.io:443/v3/order/cancel/multi
        /// </summary>
        /// <param name="orderIds">list of order ids, e.g."order_ids": [1000000, 1000001]</param>
        /// <returns></returns>
        Task<WebCallResult<List<KunaPlacedOrder>>> CancelOrdersAsync(List<long> orderIds, CancellationToken ct = default);
        /// <summary>
        /// GET https://api.kuna.io:443/v3/currencies
        /// </summary>
        /// <param name="privileged"></param>
        /// <returns></returns>
        Task<WebCallResult<List<KunaCurrency>>> GetCurrenciesAsync(bool? privileged = null, CancellationToken ct = default);
        Task<WebCallResult<KunaOrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default);
       

        Task<WebCallResult<KunaPlacedOrder>> PlaceOrderAsync(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price=null, decimal? stopPrice=null, CancellationToken ct = default);

        /// POST https://api.kuna.io:443/v3/auth/r/orders/details
        /// </summary>
        /// <param name="id"> order id</param>
        /// <returns></returns>
        Task<WebCallResult<KunaPlacedOrder>> GetOrderAsync(long id, CancellationToken ct = default);
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
        Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default);

        Task<CallResult> GetTradesHistoryToEmail(string symbol, CancellationToken ct=default);

        Task<WebCallResult<List<KunaPlacedOrder>>> GetActiveOrdersAsync(string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);

        Task<WebCallResult<List<KunaPlacedOrder>>> GetClosedOrdersAsync(string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);

        Task<WebCallResult<List<KunaPlacedOrder>>> GetOrdersWithTradesAsync(string market = null, DateTime? from = null, DateTime? to = null, bool sortDesc = true, CancellationToken ct = default);
        Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken token = default);
    }
 
}