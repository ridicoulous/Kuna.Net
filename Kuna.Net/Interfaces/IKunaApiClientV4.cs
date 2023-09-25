using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using Kuna.Net.Objects.V4;
using Kuna.Net.Objects.V4.Requests;

namespace Kuna.Net.Interfaces
{
    public interface IKunaApiClientV4 : ISpotClient
    {
        /// <summary>
        /// Rate limit value for CryptoExchange.Net.RateLimiter.RateLimiterTotal class
        /// </summary>
        int? TotalRateLimit { get; set; }

        /// <summary>
        /// Gets the server time asynchronously.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Server time.</returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        /// <summary>
        /// Not implemented yet!!!
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        Task<WebCallResult<DateTime>> GetFeeAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of tickers asynchronously.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <param name="symbols">Trading symbols to filter the tickers (optional).</param>
        /// <returns>List of tickers.</returns>
        Task<WebCallResult<IEnumerable<KunaTickerV4>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols);

        /// <summary>
        /// Gets the order book for a specified symbol asynchronously.
        /// </summary>
        /// <param name="symbol">The trading symbol (e.g., BTC_USDT).</param>
        /// <param name="level">Amount of price levels for existing orders in the response. 1000 if not provided.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Order book for the specified symbol.</returns>
        Task<WebCallResult<KunaOrderBookV4>> GetOrderBookAsync(string symbol, OrderBookLevel? level = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trading pairs asynchronously.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of trading pairs.</returns>
        Task<WebCallResult<IEnumerable<KunaTradingPairV4>>> GetTradingPairsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets available currencies and their information asynchronously.
        /// </summary>
        /// <param name="type">The currency type to filter (optional).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of currencies.</returns>
        Task<WebCallResult<IEnumerable<KunaCurrencyV4>>> GetCurrenciesAsync(CurrencyType? type = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of recent public trades asynchronously.
        /// </summary>
        /// <param name="symbol">The trading symbol (e.g., BTC_USDT).</param>
        /// <param name="limit">Number of trades to retrieve (1-100, default 25, maximum 100).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of recent public trades.</returns>
        Task<WebCallResult<IEnumerable<KunaPublicTradeV4>>> GetRecentPublicTradesAsync(string symbol, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Places an order asynchronously.
        /// </summary>
        /// <param name="parameters">Order placement request parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Information about the placed order.</returns>
        Task<WebCallResult<KunaOrderOnPlacingV4>> PlaceOrderAsync(PlaceOrderRequestV4 parameters, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order asynchronously.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to cancel.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Result of the order cancellation.</returns>
        Task<WebCallResult<KunaSimpleResponse>> CancelOrderAsync(Guid orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancels multiple orders asynchronously.
        /// </summary>
        /// <param name="orderIds">A collection of unique order identifiers to cancel.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Result of the order cancellations.</returns>
        Task<WebCallResult<IEnumerable<KunaCancelBulkOrderResponse>>> CancelOrdersAsync(IEnumerable<Guid> orderIds, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of client orders asynchronously.
        /// </summary>
        /// <param name="markets">One or more trading symbols to filter (optional).</param>
        /// <param name="from">Start date to filter orders (optional).</param>
        /// <param name="to">End date to filter orders (optional).</param>
        /// <param name="limit">Maximum number of orders to retrieve (default 100, maximum 100).</param>
        /// <param name="sortDesc">Sort the resulting list newest-on-top (true) or oldest-on-top (false).</param>
        /// <param name="status">Order status filter (optional).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of client orders.</returns>
        Task<WebCallResult<IEnumerable<KunaOrderV4>>> GetOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, KunaOrderStatusV4? status = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of client open orders asynchronously.
        /// </summary>
        /// <param name="markets">One or more trading symbols to filter (optional).</param>
        /// <param name="from">Start date to filter orders (optional).</param>
        /// <param name="to">End date to filter orders (optional).</param>
        /// <param name="limit">Maximum number of orders to retrieve (default 100, maximum 100).</param>
        /// <param name="sortDesc">Sort the resulting list newest-on-top (true) or oldest-on-top (false).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of client open orders.</returns>
        Task<WebCallResult<IEnumerable<KunaOrderV4>>> GetActiveOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, CancellationToken ct = default);

        /// <summary>
        /// Gets details of a client order asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the order to retrieve.</param>
        /// <param name="withTrades">Include trades associated with the order (optional).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Details of the client order.</returns>
        Task<WebCallResult<KunaOrderV4>> GetOrderAsync(Guid id, bool withTrades = false, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trades associated with a trading pair asynchronously.
        /// </summary>
        /// <param name="market">A trading pair as per the list from Get all traded markets endpoint.</param>
        /// <param name="orderId">UUID of an order, to receive trades for this order only (optional).</param>
        /// <param name="sortDesc">Sort the resulting list newest-on-top (true) or oldest-on-top (false).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of trades associated with the trading pair.</returns>
        Task<WebCallResult<IEnumerable<KunaUserTradeV4>>> GetTradesAsync(string market, Guid? orderId, bool sortDesc = true, CancellationToken ct = default);

        /// <summary>
        /// Gets all trades associated with an order asynchronously.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to retrieve trades for.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of trades associated with the order.</returns>
        Task<WebCallResult<IEnumerable<KunaUserTradeV4>>> GetOrderTradesAsync(Guid orderId, CancellationToken ct = default);

        /// <summary>
        /// Gets the balances of all your wallets asynchronously.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of account balances.</returns>
        Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Sets whether the account is a Pro account.
        /// </summary>
        /// <param name="isProAccountEnabled">A flag indicating whether the account is a Pro account.</param>
        void SetProAccount(bool isProAccountEnabled);
    }
 
}