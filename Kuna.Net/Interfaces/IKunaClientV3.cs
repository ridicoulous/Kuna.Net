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
        CallResult<KunaTimestampResponse> GetServerTime();
        Task<CallResult<KunaTimestampResponse>> GetServerTimeAsync(CancellationToken ct = default);
        CallResult<KunaTradingPair> GetTradingPairs();
        Task<CallResult<KunaTradingPair>> GetTradingPairsAsync( CancellationToken ct = default);
        /// <summary>
        /// Get tickers data
        /// Get https://api.kuna.io:443/v3/tickers
        /// </summary>
        /// <param name="symbols">list of explicit market codes</param>
        /// <returns></returns>
        CallResult<IEnumerable<KunaTicker>> GetTickers(params string[] symbols);

        /// <summary>
        /// Get tickers data
        /// Get https://api.kuna.io:443/v3/tickers
        /// </summary>
        /// <param name="symbols">list of explicit market codes</param>
        /// <returns></returns>
        Task<CallResult<IEnumerable<KunaTicker>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols);
        Task<CallResult<List<KunaPlacedOrder>>> GetOrdersAsync(KunaOrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);
        CallResult<List<KunaPlacedOrder>> GetOrders(KunaOrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null);

        /// <summary>
        /// Cancel multiple orders
        ///  Post https://api.kuna.io:443/v3/order/cancel/multi
        /// </summary>
        /// <param name="orderIds">list of order ids, e.g."order_ids": [1000000, 1000001]</param>
        /// <returns></returns>
        Task<CallResult<List<KunaPlacedOrder>>> CancelOrdersAsync(List<long> orderIds, CancellationToken ct = default);
        CallResult<List<KunaPlacedOrder>> CancelOrders(List<long> orderIds);
        CallResult<List<KunaTrade>> GetOrderTrades(string market, long id);
        Task<CallResult<List<KunaTrade>>> GetOrderTradesAsync(string market, long id, CancellationToken ct);
        /// <summary>
        /// GET https://api.kuna.io:443/v3/currencies
        /// </summary>
        /// <param name="privileged"></param>
        /// <returns></returns>
        CallResult<List<KunaCurrency>> GetCurrencies(bool? privileged = null);
        Task<CallResult<List<KunaTrade>>> GetCurrenciesAsync(bool? privileged = null, CancellationToken ct = default);
        CallResult<KunaOrderBook> GetOrderBook(string symbol);
        Task<CallResult<KunaOrderBook>> GetOrderBookV2Async(string symbol, CancellationToken ct = default);
        
    }
}