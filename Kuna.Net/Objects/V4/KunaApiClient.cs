using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using Kuna.Net.Helpers;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V4.Requests;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Objects.V4
{
    public class KunaV4RestApiClient : KunaBaseRestApiClient, IKunaApiClientV4
    {
        internal static readonly KeyValuePair<string, string> ProParameter = new ("account", "pro");

        #region endpoints

        private const string ServerTimeEndpoint = "public/timestamp";
        private const string CurrenciesEndpoint = "public/currencies";
        private const string TickersEndpoint = "markets/public/tickers";
        private const string TickersEndpointAuth = "markets/private/tickers";
        private const string OrderBookEndpoint = "order/public/book/{pair}";
        private const string OrderBookEndpointAuth = "order/private/book/{pair}";
        // private const string FeeEndpoint = "/public/fees"; // not implemented yet
        private const string WalletEndpoint = "private/getBalance";
        private const string CancelMultipleOrderEndpoint = "order/private/cancel/multi";
        private const string PlaceOrderEndpoint = "order/private/create";
        private const string CancelOrderEndpoint = "order/private/cancel";
        private const string ActiveOrdersEndpoint = "order/private/active";
        private const string OrdersEndpoint = "order/private/history";
        private const string TradingPairsEndpoint = "markets/public/getAll";
        private const string TradingPairsEndpointAuth = "markets/private/getAll";
        private const string OrderDetailsEndpoint = "order/private/details/{id}";

        private const string PublicTradesEndPoint = "trade/public/book/{pairs}";
        private const string PublicTradesEndPointAuth = "trade/private/book/{pairs}";
        private const string UserTradesEndPoint = "trade/private/history";
        private const string OrderTradesEndPoint = "order/private/{id}/trades";
        // private const string NotImplementedYet0 = "private/me";
        #endregion        
        private bool isProAccount;
        // private readonly bool useSingleApiKey;
        private const int ProTotalRateLimit = 1200;
        private const int RegularTotalRateLimit = 300;
        private int? userDefinedTotalRateLimit = null;
        internal static TimeSyncState TimeSyncState = new ("kuna-api");
        public event Action<string> OnError;
        public event Action<OrderId> OnOrderPlaced;
        public event Action<OrderId> OnOrderCanceled;

        public KunaV4RestApiClient(ILogger logger, HttpClient? httpClient, string baseAddress, KunaRestOptions options, KunaApiClientOptions apiOptions)
            : base(logger, httpClient, baseAddress, options, apiOptions)
        {
            isProAccount=options.IsProAccount;
            UpdateRateLimiters();
            // OnError += HandleProAccountEndpointError; //unsuported with v4
            versionSuffix = "v4";
            // useSingleApiKey = options.UseSingleApiKey;
        }

        /// <summary>
        /// Rate limit value for CryptoExchange.Net.RateLimiter.RateLimiterTotal class
        /// </summary>
        public int? TotalRateLimit
        {
            get
            {
                return userDefinedTotalRateLimit ?? (isProAccount ? ProTotalRateLimit : RegularTotalRateLimit);
            }
            set
            {
                userDefinedTotalRateLimit = value;
                UpdateRateLimiters();
            }
        }


        public override TimeSyncInfo GetTimeSyncInfo() => new (_logger, false, TimeSpan.FromSeconds(600), TimeSyncState);

        public override TimeSpan? GetTimeOffset()
            => TimeSyncState.TimeOffset;

        protected override async Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        {
            return await GetServerTimeAsync();
        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            return new KunaAuthenticationProvider(credentials, ((KunaRestOptions) ClientOptions).UseSingleApiKey);
        }

        #region Public API
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            WebCallResult<KunaTimestampResponse> tmpResult = await SendRequestAsync<KunaTimestampResponse>(ServerTimeEndpoint, HttpMethod.Get, null, false, ct);

            return tmpResult.As(tmpResult.Data.CurrentTime);
        }
        /// <summary>
        /// Not implemented yet!!!
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<WebCallResult<DateTime>> GetFeeAsync(CancellationToken ct = default)
        {
            throw new NotSupportedException();
        }

        public async Task<WebCallResult<IEnumerable<KunaTickerV4>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols)
        {
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("pairs", symbols.AsStringParameterOrNull());
            return await SendRequestAsync<IEnumerable<KunaTickerV4>>(CanBeSigned ? TickersEndpointAuth : TickersEndpoint, HttpMethod.Get, request, CanBeSigned, ct);

        }

        /// <summary>
        /// Get a list of order prices by pair (e.g. BTC_USDT)
        /// </summary>
        /// <param name="symbol">A trading pair</param>
        /// <param name="level">Amount of price levels for existing orders in the response. 1000 if not provided. </param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<KunaOrderBookV4>> GetOrderBookAsync(string symbol, OrderBookLevel? level = null, CancellationToken ct = default)
        {
            string url = CanBeSigned ? OrderBookEndpointAuth : OrderBookEndpoint;
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("level", (int?) level);
            return await SendRequestAsync<KunaOrderBookV4>(FillPathParameter(url, symbol), HttpMethod.Get, request, CanBeSigned, ct);
        }

        /// <summary>
        /// Get the list of all the traded pairs on Kuna exchange. 
        /// Returns a list of objects, each with the pair string, 
        /// the tickers of both base and quoted asset alongside their precisions.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaTradingPairV4>>> GetTradingPairsAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<KunaTradingPairV4>>(CanBeSigned ? TradingPairsEndpointAuth : TradingPairsEndpoint, HttpMethod.Get, null, CanBeSigned, ct);
        }

        /// <summary>
        /// Get available currencies and their information.
        /// If you do not specify the type, you will get information about all currencies.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaCurrencyV4>>> GetCurrenciesAsync(CurrencyType? type = null, CancellationToken ct = default)
        {
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("type", type);
            return await SendRequestAsync<IEnumerable<KunaCurrencyV4>>(CurrenciesEndpoint, HttpMethod.Get, request, false, ct);

        }

        /// <summary>
        /// Get a list of trades by pair (e.g. BTC_USDT). Returns a list of trades,
        /// newest first, 100 trades maximum, but not more than specified by the limit parameter.
        /// </summary>
        /// <param name="symbol">required</param>
        /// <param name="limit">1-100, default 25, maximum 100</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaPublicTradeV4>>> GetRecentPublicTradesAsync(string symbol, int? limit = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("limit", limit);
            var url = CanBeSigned? PublicTradesEndPointAuth : PublicTradesEndPoint;
            return await SendRequestAsync<IEnumerable<KunaPublicTradeV4>>(FillPathParameter(url, symbol), HttpMethod.Get, parameters, CanBeSigned, ct);

        }

        #endregion Public API
        #region Trading API
        public async Task<WebCallResult<KunaOrderOnPlacingV4>> PlaceOrderAsync(PlaceOrderRequestV4 parameters, CancellationToken ct = default)
        {

            var result = await SendRequestAsync<KunaOrderOnPlacingV4>(PlaceOrderEndpoint, HttpMethod.Post, parameters.AsDictionary(), true, ct);
            if (result.Success)
            {
                OnOrderPlaced?.Invoke(new OrderId() { SourceObject = result.Data, Id = result.Data.Id.ToString() });
            }
            return result;
        }

        public async Task<WebCallResult<KunaSimpleResponse>> CancelOrderAsync(Guid orderId, CancellationToken ct = default)
        {
            var result = await SendRequestAsync<KunaSimpleResponse>(CancelOrderEndpoint, HttpMethod.Post, new Dictionary<string, object>() { { "orderId", orderId } }, true, ct);
            if (result.Success)
            {
                OnOrderCanceled?.Invoke(new OrderId() { SourceObject = result.Data, Id = orderId.ToString() });
            }
            return result;
        }


        public async Task<WebCallResult<IEnumerable<KunaCancelBulkOrderResponse>>> CancelOrdersAsync(IEnumerable<Guid> orderIds, CancellationToken ct = default)
        {
            var result = await SendRequestAsync<IEnumerable<KunaCancelBulkOrderResponse>>(CancelMultipleOrderEndpoint,
                HttpMethod.Post, new Dictionary<string, object>()
                {
                    { "orderIds", orderIds }
                },
                true,
                ct);
            if (result.Success)
            {
                foreach (var order in result.Data.Where(o => o.Success))
                {
                    OnOrderCanceled?.Invoke(new OrderId() { SourceObject = order, Id = order.OrdId.ToString() });
                }
            }
            return result;
        }

        /// <summary>
        /// Get a list of client orders by pair (e.g. BTC_USDT).
        /// Returns a list of order objects, newest first, 100 orders maximum, 
        /// but not more than specified by the limit parameter.
        /// </summary>
        /// <param name="markets">one or more sumbols, all if not specified</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit">default 100, maximum 100</param>
        /// <param name="sortDesc"></param>
        /// <param name="status">Order status. By default, orders with all statuses except Open are returned. 
        /// To get active orders, please use GetActiveOrdersAsync() method.</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaOrderV4>>> GetOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, KunaOrderStatusV4? status = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            if (markets?.Any() == true)
                parameters.AddParameter("pairs", markets);

            if (sortDesc == false)
                parameters.AddParameter("sort", "asc");

            parameters.AddOptionalParameter("limit", limit);
            parameters.AddOptionalParameter("start", from);
            parameters.AddOptionalParameter("end", to);
            parameters.AddOptionalParameter("status", status);
            return await SendRequestAsync<IEnumerable<KunaOrderV4>>(OrdersEndpoint, HttpMethod.Get, parameters, true, ct);

        }

        /// <summary>
        /// Get a list of client open orders by pair (e.g. BTC_USDT).
        /// Returns an array of order objects, newest first, 100 orders maximum, 
        /// but not more than specified by the limit parameter.
        /// </summary>
        /// <param name="markets">one or more sumbols, all if not specified</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit">default 100, maximum 100</param>
        /// <param name="sortDesc"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaOrderV4>>> GetActiveOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            if (markets?.Any() == true)
                parameters.AddParameter("pairs", markets);
            
            if (sortDesc == false)
                parameters.AddParameter("sort", "asc");
            
            parameters.AddOptionalParameter("limit", limit);
            parameters.AddOptionalParameter("start", from);
            parameters.AddOptionalParameter("end", to);
            return await SendRequestAsync<IEnumerable<KunaOrderV4>>(ActiveOrdersEndpoint, HttpMethod.Get, parameters, true, ct);

        }

        /// <summary>
        /// Get order details by order id (e.g. 4802bc23-45e9-4299-b878-48a41432ef17).
        /// Returns an order object. Also, may include an array of trades if withTrades = true.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withTrades"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<KunaOrderV4>> GetOrderAsync(Guid id, bool withTrades = false, CancellationToken ct = default)
        {
            Dictionary<string, object> parameters = null;
            if (withTrades == true)
            {
                new Dictionary<string, object>();
                parameters.AddParameter("withTrades", "true");
            }
            return await SendRequestAsync<KunaOrderV4>(FillPathParameter(OrderDetailsEndpoint, id.ToString()), HttpMethod.Get, parameters, true, ct);

        }

        /// <summary>
        /// Get a list of trades by pair (e.g. BTC_USDT). Returns an array of your trades with details on each trade, 10 trades maximum, newest first.
        /// </summary>
        /// <param name="market">A trading pair as per the list from Get all traded markets endpoint.</param>
        /// <param name="orderId">UUID of an order, to receive trades for this order only.</param>
        /// <param name="sortDesc">Sort the resulting list newest-on-top (true) or oldest-on-top (false).</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaUserTradeV4>>> GetTradesAsync(string market, Guid? orderId, bool sortDesc = true, CancellationToken ct = default)
        {
            Dictionary<string, object> parameters = new();
            parameters.AddOptionalParameter("pair", market);
            parameters.AddOptionalParameter("orderId", orderId);
            if (!sortDesc)
                parameters.AddOptionalParameter("sort", "asc");

            return await SendRequestAsync<IEnumerable<KunaUserTradeV4>>(UserTradesEndPoint, HttpMethod.Get, parameters, true, ct);

        }

        /// <summary>
        /// Get all the trades, associated with the order id. Returns an array of trades, if any were executed per the order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaUserTradeV4>>> GetOrderTradesAsync(Guid orderId, CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<KunaUserTradeV4>>(FillPathParameter(OrderTradesEndPoint, orderId.ToString()), HttpMethod.Get, null, true, ct);
        }
        #endregion Trading API

        /// <summary>
        /// Review account wallets balance. Returns an array of the balances of all your wallets.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await SendRequestAsync<IEnumerable<KunaAccountBalance>>(WalletEndpoint, HttpMethod.Get, null, true, ct);
        }

        public void SetProAccount(bool isProAccountEnabled)
        {
            isProAccount = isProAccountEnabled;
            UpdateRateLimiters();
        }

        // private void HandleProAccountEndpointError(string errorMessage)
        // {
        //     if (string.IsNullOrEmpty(errorMessage))
        //     {
        //         return;
        //     }
        //     if (errorMessage.Contains("for_pro_members_only", StringComparison.OrdinalIgnoreCase))
        //     {
        //         SetProAccount(false);
        //         //log.Write(LogLevel.Warning, "You are not Pro, forcibly turn off the Pro endpoints");
        //     }
        // }

        private void UpdateRateLimiters()
        {
            ApiOptions.RateLimiters.Clear();
            ApiOptions.RateLimiters.Add(
                    new RateLimiter()
                    .AddTotalRateLimit(TotalRateLimit.Value, TimeSpan.FromMinutes(1))
                    .AddPartialEndpointLimit("/public/", 60, TimeSpan.FromMinutes(1), ignoreOtherRateLimits: true)
            );
        }

        private async Task<WebCallResult<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, Dictionary<string, object> parameters, bool signed, CancellationToken ct)
        where T : class
        {
            Dictionary<string, string> header = null;
            if (signed && isProAccount) 
            {
                header = new() { [ProParameter.Key] = ProParameter.Value };
            }
            var result = await SendRequestAsync<KunaV4BaseResponse<T>>(GetUrl(endpoint), method, ct, parameters, signed, additionalHeaders: header).ConfigureAwait(false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result.As(result.Data?.Data);
        }

        async Task<WebCallResult<OrderId>> ISpotClient.PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price, string accountId, string clientOrderId, CancellationToken ct)
        {
            var kunaOrdSide = side == CommonOrderSide.Buy ? KunaOrderSideV4.Buy : KunaOrderSideV4.Sell;
            PlaceOrderRequestV4 reqparam = type switch
            {
                CommonOrderType.Limit => PlaceOrderRequestV4.LimitOrder(symbol, quantity, kunaOrdSide, price.Value, clientOrderId == null ? null : Guid.Parse(clientOrderId)),
                CommonOrderType.Market => PlaceOrderRequestV4.MarketOrder(symbol, quantity, kunaOrdSide, id: clientOrderId == null ? null : Guid.Parse(clientOrderId)),
                _ => throw new ArgumentException($"Unsupported order type {type}")
            };
            var result = await PlaceOrderAsync(reqparam, ct);
            return WebCallResultMappings.Map(result, resp  => new OrderId() { Id = resp.Data?.Id.ToString(), SourceObject = resp.Data });
        }


        string IBaseRestClient.GetSymbolName(string baseAsset, string quoteAsset)
        {
            return $"{baseAsset}_{quoteAsset}";
        }

        async Task<WebCallResult<IEnumerable<Symbol>>> IBaseRestClient.GetSymbolsAsync(CancellationToken ct)
        {
            return WebCallResultMappings.Map(await GetTradingPairsAsync(),
                                             resp => resp.Data?.Select(p => new Symbol()
                                             {
                                                 SourceObject = p,
                                                 Name = p.Pair,
                                                 MinTradeQuantity = (decimal)Math.Pow(0.1, p.BaseAsset.Precision),// this isn't always true
                                                 PriceDecimals = p.QuoteAsset.Precision,
                                                 PriceStep = (decimal)Math.Pow(0.1, p.QuoteAsset.Precision),
                                                 QuantityDecimals = p.BaseAsset.Precision,
                                                 QuantityStep = (decimal)Math.Pow(0.1, p.BaseAsset.Precision),
                                             }));
        }


        async Task<WebCallResult<Ticker>> IBaseRestClient.GetTickerAsync(string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(await GetTickersAsync(ct, symbol),
                                              resp => resp.Data?.Select(t => new Ticker()
                                              {
                                                  SourceObject = t,
                                                  Symbol = t.Pair,
                                                  HighPrice = t.High,
                                                  LowPrice = t.Low,
                                                  Price24H = t.Price - t.PriceChange,
                                                  Volume = t.Volume,
                                                  LastPrice = t.Price,
                                              }).FirstOrDefault());
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct)
        {
            return WebCallResultMappings.Map(await GetTickersAsync(ct),
                                             resp => resp.Data?.Select(t => new Ticker()
                                             {
                                                 SourceObject = t,
                                                 Symbol = t.Pair,
                                                 HighPrice = t.High,
                                                 LowPrice = t.Low,
                                                 Price24H = t.Price - t.PriceChange,
                                                 Volume = t.Volume,
                                                 LastPrice = t.Price,
                                             }));
        }

        Task<WebCallResult<IEnumerable<Kline>>> IBaseRestClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime, DateTime? endTime, int? limit, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(
                await GetOrderBookAsync(symbol, null, ct),
                resp => new OrderBook()
                {
                    SourceObject = resp.Data,
                    Asks = resp.Data.Asks.OrderBy(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                    Bids = resp.Data.Bids.OrderByDescending(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                });
        }

        async Task<WebCallResult<IEnumerable<Trade>>> IBaseRestClient.GetRecentTradesAsync(string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(
                await GetRecentPublicTradesAsync(symbol, ct: ct),
                resp => resp.Data?.Select(t => new Trade()
                {
                    SourceObject = t,
                    Symbol = t.Pair,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    Timestamp = t.CreatedAt.DateTime,
                }));
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string accountId, CancellationToken ct)
        {
            var balances = await GetBalancesAsync();
            static IEnumerable<Balance> map(WebCallResult<IEnumerable<KunaAccountBalance>> x) => x.Data.Select(b => new Balance()
            {
                SourceObject = b,
                Asset = b.Currency,
                Available = b.Balance - b.LockBalance,
                Total = b.Balance
            });
            return WebCallResultMappings.Map(balances, map);
        }

        async Task<WebCallResult<Order>> IBaseRestClient.GetOrderAsync(string orderId, string symbol, CancellationToken ct)
        {
            var order = await GetOrderAsync(Guid.Parse(orderId));
            static Order map(WebCallResult<KunaOrderV4> x) => x.Data?.ConvertToCryptoExchangeOrder();
            return WebCallResultMappings.Map(order, map);
        }

        async Task<WebCallResult<IEnumerable<UserTrade>>> IBaseRestClient.GetOrderTradesAsync(string orderId, string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(
                await GetOrderTradesAsync(Guid.Parse(orderId), ct: ct),
                resp => resp.Data?.Select(t => new UserTrade()
                {
                    SourceObject = t,
                    Fee = t.Fee,
                    FeeAsset = t.FeeCurrency,
                    Id = t.Id.ToString(),
                    OrderId = t.OrderId.ToString(),
                    Price = t.Price,
                    Quantity = t.Quantity,
                    Symbol = t.Pair,
                    Timestamp = t.CreatedAt.DateTime
                })
            );  
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetOpenOrdersAsync(string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(
                await GetActiveOrdersAsync(new[] { symbol }, ct: ct),
                resp => resp.Data?.Select(t => t.ConvertToCryptoExchangeOrder())
            );
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetClosedOrdersAsync(string symbol, CancellationToken ct)
        {
            return WebCallResultMappings.Map(
                await GetOrdersAsync(new[] { symbol }, status: KunaOrderStatusV4.Closed, ct: ct),
                resp => resp.Data?.Select(t => t.ConvertToCryptoExchangeOrder())
            );
        }

        async Task<WebCallResult<OrderId>> IBaseRestClient.CancelOrderAsync(string orderId, string symbol, CancellationToken ct)
        {
            var result = await CancelOrderAsync(Guid.Parse(orderId), ct);
            return WebCallResultMappings.Map(result, resp => new OrderId() { Id = orderId, SourceObject = resp.Data });
        }
    }
}
