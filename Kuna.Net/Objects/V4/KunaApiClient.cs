using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Kuna.Net.Converters;
using Kuna.Net.Helpers;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V3;
using Kuna.Net.Objects.V4.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Objects.V4
{
    public class KunaV4ApiClient : KunaBaseApiClient, IKunaApiClientV4
    {
        internal static readonly KeyValuePair<string, object> IsProParameter = new ("isPro", "true");

        #region endpoints

        private const string ServerTimeEndpoint = "public/timestamp";
        private const string CurrenciesEndpoint = "public/currencies";
        private const string TickersEndpoint = "markets/public/tickers";
        private const string TickersEndpointAuth = "markets/private/tickers";
        private const string OrderBookEndpoint = "order/public/book/{pair}";
        private const string OrderBookEndpointAuth = "order/public/book/{pair}";
        private const string FeeEndpoint = "/public/fees"; // not implemented yet
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
        // private const string ProOrdersEndpoint = "auth/pro/r/orders";
        private const string NotImplementedYet0 = "private/me";
        // private const string CandlesHistoryEndpoint = "tv/history";
        #endregion        
        private bool IsProAccount;
        private const int ProTotalRateLimit = 1200;
        private const int RegularTotalRateLimit = 600;
        private int? userDefinedTotalRateLimit = null;
        internal static TimeSyncState TimeSyncState = new TimeSyncState("kuna-api");
        public event Action<string> OnError;
        public event Action<OrderId> OnOrderPlaced;
        public event Action<OrderId> OnOrderCanceled;
        private readonly Log _log;

        public KunaV4ApiClient(Log log, KunaClient baseClient, KunaClientOptions options, KunaApiClientOptions apiOptions) : base(options, apiOptions)
        {
            _log = log;
            _kunaClient = baseClient;
            IsProAccount=options.IsProAccount;
            UpdateRateLimiters();
            OnError += HandleProAccountEndpointError;
            versionSuffix = "v4";

        }

        /// <summary>
        /// Rate limit value for CryptoExchange.Net.RateLimiter.RateLimiterTotal class
        /// </summary>
        public int? TotalRateLimit
        {
            get
            {
                return userDefinedTotalRateLimit ?? (IsProAccount ? ProTotalRateLimit : RegularTotalRateLimit);
            }
            set
            {
                userDefinedTotalRateLimit = value;
                UpdateRateLimiters();
            }
        }


        public override TimeSyncInfo GetTimeSyncInfo() => new (_log, false, TimeSpan.FromSeconds(600), TimeSyncState);

        public override TimeSpan GetTimeOffset()
            => TimeSyncState.TimeOffset;

        protected override async Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        {
            return await GetServerTimeAsync();
        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            return new KunaAuthenticationProvider(credentials);
        }

        #region Public API
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            WebCallResult<KunaTimestampResponse> tmpResult = await SendRequestAsync<KunaTimestampResponse>(ServerTimeEndpoint, HttpMethod.Get, null, false, ct);

            return new WebCallResult<DateTime>(tmpResult.ResponseStatusCode, tmpResult.ResponseHeaders, null, null, null, null, null, null, tmpResult.Data.CurrentTime, tmpResult.Error);
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
            // return WebCallResultMappings.Map(result, x => new KunaOrderBookV4(x.Data));
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
        public async Task<WebCallResult<List<KunaCurrencyV4>>> GetCurrenciesAsync(CurrencyType? type = null, CancellationToken ct = default)
        {
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("type", type);
            return await SendRequestAsync<List<KunaCurrencyV4>>(CurrenciesEndpoint, HttpMethod.Get, request, false, ct);

        }

        /// <summary>
        /// Get a list of trades by pair (e.g. BTC_USDT). Returns a list of trades,
        /// newest first, 100 trades maximum, but not more than specified by the limit parameter.
        /// </summary>
        /// <param name="symbol"></param>
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


        public async Task<WebCallResult<List<KunaCancelBulkOrderResponse>>> CancelOrdersAsync(IEnumerable<Guid> orderIds, CancellationToken ct = default)
        {
            var result = await SendRequestAsync<List<KunaCancelBulkOrderResponse>>(CancelMultipleOrderEndpoint,
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
        public async Task<WebCallResult<List<KunaOrderV4>>> GetOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, KunaOrderStatusV4? status = null, CancellationToken ct = default)
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
            return await SendRequestAsync<List<KunaOrderV4>>(OrdersEndpoint, HttpMethod.Get, parameters, true, ct);

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
        public async Task<WebCallResult<List<KunaOrderV4>>> GetActiveOrdersAsync(IEnumerable<string> markets = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool sortDesc = true, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            if (markets?.Any() == true)
                parameters.AddParameter("pairs", markets);
            
            if (sortDesc == false)
                parameters.AddParameter("sort", "asc");
            
            parameters.AddOptionalParameter("limit", limit);
            parameters.AddOptionalParameter("start", from);
            parameters.AddOptionalParameter("end", to);
            return await SendRequestAsync<List<KunaOrderV4>>(ActiveOrdersEndpoint, HttpMethod.Get, parameters, true, ct);

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
        public async Task<WebCallResult<List<KunaTradeV4>>> GetTradesAsync(string market, Guid? orderId, bool sortDesc = true, CancellationToken ct = default)
        {
            Dictionary<string, object> parameters = new();
            parameters.AddOptionalParameter("pair", market);
            parameters.AddOptionalParameter("orderId", orderId);
            if (!sortDesc)
                parameters.AddOptionalParameter("sort", "asc");

            return await SendRequestAsync<List<KunaTradeV4>>(UserTradesEndPoint, HttpMethod.Get, parameters, true, ct);

        }

        /// <summary>
        /// Get all the trades, associated with the order id. Returns an array of trades, if any were executed per the order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<KunaTradeV4>>> GetOrderTradesAsync(Guid orderId, CancellationToken ct = default)
        {
            return await SendRequestAsync<List<KunaTradeV4>>(FillPathParameter(OrderTradesEndPoint, orderId.ToString()), HttpMethod.Get, null, true, ct);
        }
        #endregion Trading API

        // public WebCallResult<IEnumerable<KunaAccountBalance>> GetBalances() => GetBalancesAsync().Result;
        // public async Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default)
        // {
        //     string url = IsProAccount ? ProWalletsEndpoint : WalletEndpoint;
        //     return await SendRequestAsync<IEnumerable<KunaAccountBalance>>(GetUrl(url), HttpMethod.Post, ct, new Dictionary<string, object>(), true);

        // }

        // public async Task<CallResult> GetTradesHistoryToEmail(string symbol, CancellationToken ct = default)
        // {
        //     var request = await SendRequestAsync<object>(GetUrl("auth/history/trades"), HttpMethod.Post, default, new Dictionary<string, object>() { { "market", symbol } }, true);
        //     return request;
        // }

        // public async Task<WebCallResult<OrderId>> PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price = null, string accountId = null, string clientOrderId = null, CancellationToken ct = default)
        // {

        //     var kunaSide = side == CommonOrderSide.Sell ? KunaOrderSide.Sell : KunaOrderSide.Buy;
        //     var kunaType = type == CommonOrderType.Limit ? KunaOrderType.Limit : KunaOrderType.Market;
        //     var order = await PlaceOrderAsync(symbol, kunaSide, kunaType, quantity, price, null, ct);

        //     Func<WebCallResult<KunaPlacedOrder>, OrderId> map = x => new OrderId() { Id = x.Data.Id.ToString(), SourceObject = x.Data };

        //     return WebCallResultMappings.Map(order, map);
        // }

        // public string GetSymbolName(string baseAsset, string quoteAsset)
        // {
        //     return baseAsset.ToLower() + quoteAsset.ToLower();
        // }

        // public async Task<WebCallResult<IEnumerable<Symbol>>> GetSymbolsAsync(CancellationToken ct = default)
        // {
        //     var markets = await GetTradingPairsAsync();

        //     Func<WebCallResult<IEnumerable<KunaTradingPair>>, IEnumerable<Symbol>> map = x => x.Data.Select(x => new Symbol()
        //     {
        //         MinTradeQuantity = x.CommonMinimumTradeSize,
        //         Name = x.CommonName,
        //         PriceDecimals = 1 / x.QuotePrecision,
        //         PriceStep = x.CommonMinimumTradeSize,
        //         QuantityDecimals = 1 / x.BasePrecision,
        //         QuantityStep = x.CommonMinimumTradeSize,
        //         SourceObject = x
        //     }).ToList();

        //     return WebCallResultMappings.Map(markets, map);
        // }

        // public async Task<WebCallResult<Ticker>> GetTickerAsync(string symbol, CancellationToken ct = default)
        // {
        //     var result = await GetTickersAsync(default, symbol);
        //     Func<WebCallResult<IEnumerable<KunaTicker>>, Ticker> map = x => new Ticker()
        //     {
        //         SourceObject = x,
        //         HighPrice = x.Data.FirstOrDefault()?.CommonHigh ?? 0,
        //         LastPrice = x.Data.FirstOrDefault()?.LastPrice ?? 0,
        //         LowPrice = x.Data.FirstOrDefault()?.Low ?? 0,
        //         Price24H = x.Data.FirstOrDefault()?.PriceDiffPercent,
        //         Symbol = x.Data.FirstOrDefault()?.Symbol,
        //         Volume = x.Data.FirstOrDefault()?.Volume
        //     };
        //     return WebCallResultMappings.Map(result, map);
        // }

        // async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct = default)
        // {
        //     var result = await this.GetTickersAsync(ct);
        //     Func<WebCallResult<IEnumerable<KunaTicker>>, IEnumerable<Ticker>> map = t => t.Data.Select(x => new Ticker()
        //     {
        //         SourceObject = x,
        //         HighPrice = x.CommonHigh,
        //         LastPrice = x.LastPrice,
        //         LowPrice = x.Low,
        //         Price24H = x.PriceDiffPercent,
        //         Symbol = x.Symbol,
        //         Volume = x.Volume
        //     }).ToList();
        //     return result.As<IEnumerable<Ticker>>(result.Data.Select(x => new Ticker()
        //     {
        //         SourceObject = x,
        //         HighPrice = x.CommonHigh,
        //         LastPrice = x.LastPrice,
        //         LowPrice = x.Low,
        //         Price24H = x.LastPrice,
        //         Symbol = x.Symbol,
        //         Volume = x.Volume
        //     }));
        // }


        // public Task<WebCallResult<IEnumerable<Kline>>> GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        // {
        //     throw new NotImplementedException();
        // }

        // async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct = default)
        // {
        //     var book = await GetOrderBookAsync(symbol);
        //     Func<WebCallResult<KunaOrderBook>, OrderBook> map = x => new OrderBook()
        //     {
        //         Asks = x.Data.Asks.OrderBy(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
        //         Bids = x.Data.Bids.OrderByDescending(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
        //         SourceObject = x.Data,
        //     };
        //     return WebCallResultMappings.Map(book, map);
        // }

        // public Task<WebCallResult<IEnumerable<Trade>>> GetRecentTradesAsync(string symbol, CancellationToken ct = default)
        // {
        //     throw new NotImplementedException();
        // }

        // async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string accountId = null, CancellationToken ct = default)
        // {
        //     var balances = await GetBalancesAsync();
        //     Func<WebCallResult<IEnumerable<KunaAccountBalance>>, IEnumerable<Balance>> map = x => x.Data.Select(b => new Balance()
        //     {
        //         Asset = b.CommonAsset,
        //         Available = b.Available,
        //         SourceObject = b,
        //         Total = b.Total
        //     });
        //     return WebCallResultMappings.Map(balances, map);
        // }

        // public async Task<WebCallResult<Order>> GetOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
        // {
        //     long id = 0;
        //     if (long.TryParse(orderId, out id))
        //     {
        //         var order = await GetOrderAsync(id);
        //         Func<WebCallResult<KunaPlacedOrder>, Order> map = x => new Order()
        //         {
        //             Id = x.Data.Id.ToString(),
        //             Price = x.Data.Price,
        //             Quantity = x.Data.AmountPlaced,
        //             QuantityFilled = x.Data.AmountExecuted,
        //             Side = x.Data.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
        //             SourceObject = x.Data,
        //             Status = x.Data.Status switch
        //             {

        //                 KunaOrderStatus.Active => CommonOrderStatus.Active,
        //                 KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
        //                 KunaOrderStatus.Filled => CommonOrderStatus.Filled,
        //                 KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
        //                 _ => CommonOrderStatus.Canceled

        //             },
        //             Symbol = x.Data.Symbol,
        //             Timestamp = x.Data.TimestampCreated,
        //             Type = x.Data.Type switch
        //             {
        //                 KunaOrderType.Limit => CommonOrderType.Limit,
        //                 KunaOrderType.Market => CommonOrderType.Market,
        //                 KunaOrderType.MarketByQuote => CommonOrderType.Market,
        //                 KunaOrderType.StopLimit => CommonOrderType.Other,
        //                 _ => CommonOrderType.Other
        //             },

        //         };
        //         return WebCallResultMappings.Map(order, map);
        //     }
        //     return new WebCallResult<Order>(new ServerError($"Can not parse id {orderId}"));

        // }

        // public async Task<WebCallResult<IEnumerable<UserTrade>>> GetOrderTradesAsync(string orderId, string symbol = null, CancellationToken ct = default)
        // {
        //     long id = 0;
        //     if (long.TryParse(orderId, out id))
        //     {
        //         var orderTrades = await GetOrderTradesAsync(symbol, id);
        //         Func<WebCallResult<List<KunaTrade>>, IEnumerable<UserTrade>> map = x => x.Data.Select(c => new UserTrade()
        //         {
        //             Fee = c.Fee,
        //             Id = c.Id.ToString(),
        //             OrderId = c.OrderId.ToString(),
        //             Price = c.ExecutedPrice,
        //             Quantity = c.ExecutedAmount,
        //             SourceObject = c,
        //             Symbol = c.Pair,
        //             Timestamp = c.CommonTradeTime
        //         });
        //     }
        //     return new WebCallResult<IEnumerable<UserTrade>>(new ServerError($"Can not parse id {orderId}"));
        // }

        // public async Task<WebCallResult<IEnumerable<Order>>> GetOpenOrdersAsync(string symbol = null, CancellationToken ct = default)
        // {
        //     var result = await GetActiveOrdersAsync(symbol);
        //     Func<WebCallResult<List<KunaPlacedOrder>>, IEnumerable<Order>> map = t => t.Data.Select(x => new Order()
        //     {
        //         SourceObject = x,
        //         Id = x.Id.ToString(),
        //         Price = x.Price,
        //         Quantity = x.AmountPlaced,
        //         QuantityFilled = x.AmountExecuted,
        //         Side = x.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
        //         Status = x.Status switch
        //         {
        //             KunaOrderStatus.Active => CommonOrderStatus.Active,
        //             KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
        //             KunaOrderStatus.Filled => CommonOrderStatus.Filled,
        //             KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
        //             _ => CommonOrderStatus.Canceled
        //         },
        //         Symbol = x.Symbol,
        //         Type = x.Type switch
        //         {
        //             KunaOrderType.Limit => CommonOrderType.Limit,
        //             KunaOrderType.Market => CommonOrderType.Market,
        //             KunaOrderType.MarketByQuote => CommonOrderType.Market,
        //             KunaOrderType.StopLimit => CommonOrderType.Other,
        //             _ => CommonOrderType.Other
        //         },
        //         Timestamp = x.TimestampCreated
        //     }).ToList();
        //     return WebCallResultMappings.Map(result, map);
        // }

        // public async Task<WebCallResult<IEnumerable<Order>>> GetClosedOrdersAsync(string symbol = null, CancellationToken ct = default)
        // {
        //     var result = await GetClosedOrdersAsync(symbol, null, null, 1000, true);
        //     Func<WebCallResult<List<KunaPlacedOrder>>, IEnumerable<Order>> map = t => t.Data.Select(x => new Order()
        //     {
        //         SourceObject = x,
        //         Id = x.Id.ToString(),
        //         Price = x.Price,
        //         Quantity = x.AmountPlaced,
        //         QuantityFilled = x.AmountExecuted,
        //         Side = x.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
        //         Status = x.Status switch
        //         {
        //             KunaOrderStatus.Active => CommonOrderStatus.Active,
        //             KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
        //             KunaOrderStatus.Filled => CommonOrderStatus.Filled,
        //             KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
        //             _ => CommonOrderStatus.Canceled
        //         }
        //     }).ToList();
        //     return WebCallResultMappings.Map(result, map);
        // }

        // public async Task<WebCallResult<OrderId>> CancelOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
        // {
        //     long id = 0;
        //     if (long.TryParse(orderId, out id))
        //     {
        //         var cancel = await CancelOrderAsync(id);
        //         Func<WebCallResult<KunaCanceledOrder>, OrderId> map = x => new OrderId() { SourceObject = x, Id = x.Data.Id.ToString() };
        //         return WebCallResultMappings.Map(cancel, map);
        //     }
        //     return new WebCallResult<OrderId>(new ServerError($"Can not parse id {orderId}"));
        // }
        // public async Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken ct = default)
        // {
        //     var parameters = new Dictionary<string, object>() {
        //         { "symbol", symbol }, { "resolution", resolution },
        //         { "from", JsonConvert.SerializeObject(from, new DateTimeConverter()) },
        //         { "to", JsonConvert.SerializeObject(to, new DateTimeConverter()) } };
        //     var result = await SendRequestAsync<TradingViewOhclvV2>(GetUrl(CandlesHistoryEndpoint), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
        //     if (result)
        //     {
        //         List<KunaOhclvV2> data = null;
        //         if (result.Success)
        //         {
        //             data = new List<KunaOhclvV2>();
        //             var t = result.Data;
        //             for (int i = 0; i < result.Data.Closes.Count; i++)
        //             {
        //                 var candle = new KunaOhclvV2(t.Timestamps[i], t.Opens[i], t.Highs[i], t.Lows[i], t.Closes[i], t.Volumes[i]);
        //                 data.Add(candle);
        //             }
        //         }
        //         return new CallResult<List<KunaOhclvV2>>(data);
        //     }
        //     return new CallResult<List<KunaOhclvV2>>(result.Error);

        // }



        public void SetProAccount(bool isProAccountEnabled)
        {
            IsProAccount = isProAccountEnabled;
            UpdateRateLimiters();
        }

        private void HandleProAccountEndpointError(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return;
            }
            if (errorMessage.Contains("for_pro_members_only", StringComparison.OrdinalIgnoreCase))
            {
                SetProAccount(false);
                //log.Write(LogLevel.Warning, "You are not Pro, forcibly turn off the Pro endpoints");
            }
        }

        private void UpdateRateLimiters()
        {
            // this.Options.RateLimiters.RemoveAll(c => c.ToString() == "TotalRateLimiter");

            this.Options.RateLimiters.Clear();
            var newLimits = new List<IRateLimiter>
                {
                    new RateLimiter()
                    .AddTotalRateLimit(TotalRateLimit.Value, TimeSpan.FromMinutes(1))
                    .AddPartialEndpointLimit("/public/", 1 , TimeSpan.FromMinutes(1), countPerEndpoint:true, ignoreOtherRateLimits: true)
                };
            foreach (var limit in newLimits)
                this.Options.RateLimiters.Add(limit);
        }

        private async Task<WebCallResult<T>> SendRequestAsync<T>(string endpoint, HttpMethod method, Dictionary<string, object> parameters, bool signed, CancellationToken ct)
        where T : class
        {
            var result = await SendRequestAsync<T>(GetUrl(endpoint), method, ct, parameters, signed).ConfigureAwait(false);

        }
    }
}
