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
using Kuna.Net.Objects.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Objects.V3
{
    public class KunaApiClient : RestApiClient, IKunaApiClientV3
    {
        #region endpoints
        private const string ServerTimeEndpoint = "timestamp";
        private const string CandlesHistoryEndpoint = "tv/history";
        private const string OrdersEndpoint = "auth/r/orders";
        private const string CurrenciesEndpoint = "currencies";
        private const string TickersEndpoint = "tickers";
        private const string OrderBookEndpoint = "book/{}";

        private const string V3PlaceOrderEndpoint = "auth/w/order/submit";
        private const string V3CancelOrderEndpoint = "order/cancel";
        private const string OrderDetailsEndpoint = "auth/r/orders/details";

        private const string TradingPairsEndpoint = "markets";
        private const string PublicTradesEndPoint = "trades/{}/hist";
        private const string WalletEndpoint = "auth/r/wallets";

        private const string ProBookEnpoint = "auth/pro/book/{}";
        private const string ProWalletsEndpoint = "auth/pro/r/wallets";
        private const string ProCancelOrderEndpoint = "auth/pro/order/cancel";
        private const string ProCancelMultipleOrdersEndpoint = "auth/pro/order/cancel/multi";
        private const string ProPlaceOrderEndpoint = "auth/pro/w/order/submit";
        private const string ProOrderDetailsEndpoint = "auth/pro/r/orders/details";

        private const string ProOrdersEndpoint = "auth/pro/r/orders";
        #endregion        
        private readonly KunaClient _kunaClient;
        public event Action<string> OnError;
        public event Action<OrderId> OnOrderPlaced;
        public event Action<OrderId> OnOrderCanceled;

        private bool IsProAccount;
        private const int ProTotalRateLimit = 1200;
        private const int RegularTotalRateLimit = 600;
        private int? userDefinedTotalRateLimit = null;
        private readonly TimeSyncInfo _timeSyncInfo;
        internal static TimeSyncState TimeSyncState = new TimeSyncState("kuna-api-v3");

        private readonly Log _log;
        public string ExchangeName => "Kuna";

        public KunaApiClient(Log log, KunaClient baseClient, KunaClientOptions options, KunaApiClientOptions apiOptions) : base(options, apiOptions)
        {
            _log = log;
            _kunaClient = baseClient;
            IsProAccount=options.IsProAccount;
            UpdateRateLimiters();
            OnError = HandleProAccountEndpointError;

        }
        protected Uri GetUrl(string endpoint)
        {
            return new Uri($"{BaseAddress}{endpoint}");
        }
        public WebCallResult<IEnumerable<KunaTicker>> GetTickers(params string[] symbols) => GetTickersAsync(default, symbols).Result;
        public async Task<WebCallResult<IEnumerable<KunaTicker>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols)
        {
            var request = new Dictionary<string, object>();
            string symb = symbols.AsStringParameterOrNull() ?? "ALL";
            request.AddOptionalParameter("symbols", symb);
            var result = await SendRequestAsync<IEnumerable<KunaTicker>>(GetUrl(TickersEndpoint), HttpMethod.Get, ct, request, false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        private async Task<WebCallResult<T>> SendRequestAsync<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object> request, bool signed, HttpMethodParameterPosition? position = null) where T : class
        {
            return await _kunaClient.SendRequestInternal<T>(this, uri, method, ct, request, signed, position);
        }
        /// <summary>
        /// Fill parameters in a path. Parameters are specified by '{}' and should be specified in occuring sequence
        /// </summary>
        /// <param name="path">The total path string</param>
        /// <param name="values">The values to fill</param>
        /// <returns></returns>
        protected static string FillPathParameter(string path, params string[] values)
        {
            foreach (var value in values)
            {
                var index = path.IndexOf("{}", StringComparison.Ordinal);
                if (index >= 0)
                {
                    path = path.Remove(index, 2);
                    path = path.Insert(index, value);
                }
            }
            return path;
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
        public WebCallResult<KunaOrderBook> GetOrderBook(string symbol) => this.GetOrderBookAsync(symbol).Result;
        public async Task<WebCallResult<KunaOrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProBookEnpoint : OrderBookEndpoint;
            var result = await SendRequestAsync<IEnumerable<KunaOrderBookEntry>>(GetUrl(FillPathParameter(url, symbol)), IsProAccount ? HttpMethod.Post : HttpMethod.Get, ct, null, IsProAccount).ConfigureAwait(false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }

            return WebCallResultMappings.Map(result, x => new KunaOrderBook(x.Data));
        }

        public WebCallResult<DateTime> GetServerTime() => GetServerTimeAsync().Result;
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var tmpResult = await SendRequestAsync<KunaTimestampResponse>(GetUrl(ServerTimeEndpoint), HttpMethod.Get, ct, null, false).ConfigureAwait(false);

            var result = new WebCallResult<DateTime>(tmpResult.ResponseStatusCode, tmpResult.ResponseHeaders, null, null, null, null, null, null, tmpResult.Data.CurrentTime, tmpResult.Error);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public WebCallResult<IEnumerable<KunaTradingPair>> GetTradingPairs() => GetTradingPairsAsync().Result;

        public async Task<WebCallResult<IEnumerable<KunaTradingPair>>> GetTradingPairsAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<IEnumerable<KunaTradingPair>>(GetUrl(TradingPairsEndpoint), HttpMethod.Get, ct, null, false).ConfigureAwait(false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public WebCallResult<List<KunaCurrency>> GetCurrencies(bool? privileged = null) => GetCurrenciesAsync(privileged, default).Result;
        public async Task<WebCallResult<List<KunaCurrency>>> GetCurrenciesAsync(bool? privileged = null, CancellationToken ct = default)
        {
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("privileged", privileged);
            var result = await SendRequestAsync<List<KunaCurrency>>(GetUrl(CurrenciesEndpoint), HttpMethod.Get, ct, request, false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }
        public WebCallResult<IEnumerable<KunaPublicTrade>> GetRecentPublicTrades(string symbol, int limit = 25) => GetRecentPublicTradesAsync(symbol, limit).Result;

        public async Task<WebCallResult<IEnumerable<KunaPublicTrade>>> GetRecentPublicTradesAsync(string symbol, int limit = 25, CancellationToken ct = default)
        {
            limit = limit > 500 ? 500 : limit;
            var parameters = new Dictionary<string, object>();
            parameters.AddParameter("limit", limit);
            var result = await SendRequestAsync<IEnumerable<KunaPublicTrade>>(GetUrl(FillPathParameter(PublicTradesEndPoint, symbol)), HttpMethod.Get, ct, parameters, false);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public async Task<WebCallResult<KunaPlacedOrder>> PlaceOrderAsync(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price = null, decimal? stopPrice = null, CancellationToken ct = default)
        {

            var amount = side switch
            {
                KunaOrderSide.Buy => Math.Abs(quantity),
                KunaOrderSide.Sell => Math.Abs(quantity) * -1,
                _ => throw new NotImplementedException("Undefined order side. Possible either Buy or Sell")
            };
            var parameters = new Dictionary<string, object>();
            parameters.AddParameter("symbol", symbol);
            parameters.AddParameter("type", JsonConvert.SerializeObject(orderType, new OrderTypeConverter()));
            parameters.AddParameter("amount", amount);
            parameters.AddOptionalParameter("price", price);
            parameters.AddOptionalParameter("stop_price", stopPrice);

            string url = IsProAccount ? ProPlaceOrderEndpoint : V3PlaceOrderEndpoint;
            var result = await SendRequestAsync<KunaPlacedOrder>(GetUrl(url), HttpMethod.Post, ct, parameters, true, HttpMethodParameterPosition.InBody);
            if (result.Success)
            {
                OnOrderPlaced?.Invoke(new OrderId() { SourceObject = result.Data, Id = result.Data.Id.ToString() });
            }
            else
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public WebCallResult<KunaPlacedOrder> PlaceOrder(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price = null, decimal? stopPrice = null) => PlaceOrderAsync(symbol, side, orderType, quantity, price, stopPrice).Result;

        public async Task<WebCallResult<KunaCanceledOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProCancelOrderEndpoint : V3CancelOrderEndpoint;
            var result = await SendRequestAsync<KunaCanceledOrder>(GetUrl(url), HttpMethod.Post, ct, new Dictionary<string, object>() { { "order_id", orderId } }, true, HttpMethodParameterPosition.InBody);
            if (result.Success)
            {
                OnOrderCanceled?.Invoke(new OrderId() { SourceObject = result.Data, Id = result.Data.Id.ToString() });
            }
            else
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public WebCallResult<KunaCanceledOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;


        public async Task<WebCallResult<List<KunaPlacedOrder>>> CancelOrdersAsync(List<long> orderIds, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProCancelMultipleOrdersEndpoint : V3CancelOrderEndpoint + "/multi";
            var result = await SendRequestAsync<List<KunaPlacedOrder>>(GetUrl(url),
                HttpMethod.Post, ct, new Dictionary<string, object>()
                {
                    { "order_ids", JsonConvert.SerializeObject(orderIds) }
                },
                true,
                HttpMethodParameterPosition.InBody);
            if (result.Success)
            {
                foreach (var order in result.Data)
                {
                    OnOrderCanceled?.Invoke(new OrderId() { SourceObject = order, Id = order.Id.ToString() });
                }
            }
            else
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }
        public WebCallResult<List<KunaPlacedOrder>> CancelOrders(List<long> orderIds) => CancelOrdersAsync(orderIds).Result;

        /// <summary>
        /// the result may be unexpected, please use one of GetActiveOrdersAsync(), GetClosedOrdersAsync(),
        /// GetOrdersWithTradesAsync() methods
        /// </summary>
        /// <param name="state"></param>
        /// <param name="market"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit"></param>
        /// <param name="sortDesc"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<WebCallResult<List<KunaPlacedOrder>>> GetOrdersAsync(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default)
        {
            var endpoint = IsProAccount ? ProOrdersEndpoint : OrdersEndpoint;
            if (!String.IsNullOrEmpty(market))
            {
                endpoint += $"/{market}";
            }
            if (state == KunaOrderStatus.Filled || state == KunaOrderStatus.Canceled)
            {
                if (!endpoint.EndsWith("/"))
                {
                    endpoint += "/";
                }
                endpoint += "hist";
            }
            var url = GetUrl(endpoint);

            var parameters = new Dictionary<string, object>();
            if (from.HasValue)
                parameters.AddOptionalParameter("start", JsonConvert.SerializeObject(from, new DateTimeConverter()));
            if (to.HasValue)
                parameters.AddOptionalParameter("end", JsonConvert.SerializeObject(to, new DateTimeConverter()));
            if (limit.HasValue)
                parameters.AddOptionalParameter("limit", limit.Value);
            if (sortDesc.HasValue)
                parameters.AddOptionalParameter("sort", sortDesc.Value ? -1 : 1);
            var result = await SendRequestAsync<List<KunaPlacedOrder>>(url, HttpMethod.Post, ct, parameters, true);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }
        public WebCallResult<List<KunaPlacedOrder>> GetOrders(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null)
        => GetOrdersAsync(state, market, from, to, limit, sortDesc).Result;

        public async Task<WebCallResult<List<KunaPlacedOrder>>> GetActiveOrdersAsync(string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default)
        => await GetOrdersAsync(KunaOrderStatus.Active, market, from, to, limit, sortDesc, ct);

        public async Task<WebCallResult<List<KunaPlacedOrder>>> GetClosedOrdersAsync(string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default)
        => await GetOrdersAsync(KunaOrderStatus.Filled, market, from, to, limit, sortDesc, ct);

        public async Task<WebCallResult<List<KunaPlacedOrder>>> GetOrdersWithTradesAsync(string market = null, DateTime? from = null, DateTime? to = null, bool sortDesc = true, CancellationToken ct = default)
        {
            var openOrders = await GetActiveOrdersAsync(market, from, to, null, sortDesc, ct);
            var closedOrders = await GetClosedOrdersAsync(market, from, to, null, sortDesc, ct);
            List<KunaPlacedOrder> orders = null;
            Error error = null;
            System.Net.HttpStatusCode? sCode;
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers;


            if (!openOrders)
            {
                error = openOrders.Error;
                sCode = openOrders.ResponseStatusCode;
                headers = openOrders.ResponseHeaders;
            }
            else if (!closedOrders)
            {
                error = closedOrders.Error;
                sCode = openOrders.ResponseStatusCode;
                headers = openOrders.ResponseHeaders;
            }
            else
            {
                orders = new List<KunaPlacedOrder>(openOrders.Data?.Where(x => x.AmountExecuted > 0));
                orders.AddRange(closedOrders.Data?.Where(x => x.AmountExecuted > 0));
                orders = sortDesc ? orders.OrderByDescending(x => x.TimestampUpdated).ToList()
                        : orders.OrderBy(x => x.TimestampUpdated).ToList();
                sCode = openOrders.ResponseStatusCode;
                headers = openOrders.ResponseHeaders;
            }
            return new WebCallResult<List<KunaPlacedOrder>>(sCode, headers, null, null, null, null, null, null, orders, error);
        }
        public WebCallResult<KunaPlacedOrder> GetOrder(long id) => GetOrderAsync(id).Result;

        public async Task<WebCallResult<KunaPlacedOrder>> GetOrderAsync(long id, CancellationToken ct = default)
        {
            var url = IsProAccount ? ProOrderDetailsEndpoint : OrderDetailsEndpoint;
            var parameters = new Dictionary<string, object>();
            parameters.AddParameter("id", id);
            var result = await SendRequestAsync<KunaPlacedOrder>(GetUrl(url), HttpMethod.Post, ct, parameters, true);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public WebCallResult<List<KunaTrade>> GetOrderTrades(string market, long id) => GetOrderTradesAsync(market, id).Result;
        public async Task<WebCallResult<List<KunaTrade>>> GetOrderTradesAsync(string market, long id, CancellationToken ct = default)
        {
            var url = GetUrl($"auth/r/order/{market}:{id}/trades");
            var result = await SendRequestAsync<List<KunaTrade>>(url, HttpMethod.Post, ct, new Dictionary<string, object>(), true);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }
        public WebCallResult<IEnumerable<KunaAccountBalance>> GetBalances() => GetBalancesAsync().Result;
        public async Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            string url = IsProAccount ? ProWalletsEndpoint : WalletEndpoint;
            var result = await SendRequestAsync<IEnumerable<KunaAccountBalance>>(GetUrl(url), HttpMethod.Post, ct, new Dictionary<string, object>(), true);
            if (!result.Success)
            {
                OnError?.Invoke(result.Error.Message);
            }
            return result;
        }

        public async Task<CallResult> GetTradesHistoryToEmail(string symbol, CancellationToken ct = default)
        {
            var request = await SendRequestAsync<object>(GetUrl("auth/history/trades"), HttpMethod.Post, default, new Dictionary<string, object>() { { "market", symbol } }, true);
            return request;
        }

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
            this.Options.RateLimiters.RemoveAll(c => c.ToString() == "TotalRateLimiter");

            this.Options.RateLimiters.Clear();
            var newLimits = new List<IRateLimiter>
                {
                    new RateLimiter()
                    .AddTotalRateLimit(TotalRateLimit.Value, TimeSpan.FromMinutes(1))
                };
            foreach (var limit in newLimits)
                this.Options.RateLimiters.Add(limit);
        }
        public override TimeSyncInfo GetTimeSyncInfo() => new TimeSyncInfo(_log, false, TimeSpan.FromSeconds(600), TimeSyncState);

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

        public async Task<WebCallResult<OrderId>> PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price = null, string accountId = null, string clientOrderId = null, CancellationToken ct = default)
        {

            var kunaSide = side == CommonOrderSide.Sell ? KunaOrderSide.Sell : KunaOrderSide.Buy;
            var kunaType = type == CommonOrderType.Limit ? KunaOrderType.Limit : KunaOrderType.Market;
            var order = await PlaceOrderAsync(symbol, kunaSide, kunaType, quantity, price, null, ct);

            Func<WebCallResult<KunaPlacedOrder>, OrderId> map = x => new OrderId() { Id = x.Data.Id.ToString(), SourceObject = x.Data };

            return WebCallResultMappings.Map(order, map);
        }

        public string GetSymbolName(string baseAsset, string quoteAsset)
        {
            return baseAsset.ToLower() + quoteAsset.ToLower();
        }

        public async Task<WebCallResult<IEnumerable<Symbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var markets = await GetTradingPairsAsync();

            Func<WebCallResult<IEnumerable<KunaTradingPair>>, IEnumerable<Symbol>> map = x => x.Data.Select(x => new Symbol()
            {
                MinTradeQuantity = x.CommonMinimumTradeSize,
                Name = x.CommonName,
                PriceDecimals = 1 / x.QuotePrecision,
                PriceStep = x.CommonMinimumTradeSize,
                QuantityDecimals = 1 / x.BasePrecision,
                QuantityStep = x.CommonMinimumTradeSize,
                SourceObject = x
            }).ToList();

            return WebCallResultMappings.Map(markets, map);
        }

        public async Task<WebCallResult<Ticker>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            var result = await GetTickersAsync(default, symbol);
            Func<WebCallResult<IEnumerable<KunaTicker>>, Ticker> map = x => new Ticker()
            {
                SourceObject = x,
                HighPrice = x.Data.FirstOrDefault()?.CommonHigh ?? 0,
                LastPrice = x.Data.FirstOrDefault()?.LastPrice ?? 0,
                LowPrice = x.Data.FirstOrDefault()?.Low ?? 0,
                Price24H = x.Data.FirstOrDefault()?.PriceDiffPercent,
                Symbol = x.Data.FirstOrDefault()?.Symbol,
                Volume = x.Data.FirstOrDefault()?.Volume
            };
            return WebCallResultMappings.Map(result, map);
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct = default)
        {
            var result = await this.GetTickersAsync(ct);
            Func<WebCallResult<IEnumerable<KunaTicker>>, IEnumerable<Ticker>> map = t => t.Data.Select(x => new Ticker()
            {
                SourceObject = x,
                HighPrice = x.CommonHigh,
                LastPrice = x.LastPrice,
                LowPrice = x.Low,
                Price24H = x.PriceDiffPercent,
                Symbol = x.Symbol,
                Volume = x.Volume
            }).ToList();
            return result.As<IEnumerable<Ticker>>(result.Data.Select(x => new Ticker()
            {
                SourceObject = x,
                HighPrice = x.CommonHigh,
                LastPrice = x.LastPrice,
                LowPrice = x.Low,
                Price24H = x.LastPrice,
                Symbol = x.Symbol,
                Volume = x.Volume
            }));
        }


        public Task<WebCallResult<IEnumerable<Kline>>> GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct = default)
        {
            var book = await GetOrderBookAsync(symbol);
            Func<WebCallResult<KunaOrderBook>, OrderBook> map = x => new OrderBook()
            {
                Asks = x.Data.Asks.OrderBy(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                Bids = x.Data.Bids.OrderByDescending(p => p.Price).Select(a => new OrderBookEntry() { Price = a.Price, Quantity = a.Quantity }),
                SourceObject = x.Data,
            };
            return WebCallResultMappings.Map(book, map);
        }

        public Task<WebCallResult<IEnumerable<Trade>>> GetRecentTradesAsync(string symbol, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string accountId = null, CancellationToken ct = default)
        {
            var balances = await GetBalancesAsync();
            Func<WebCallResult<IEnumerable<KunaAccountBalance>>, IEnumerable<Balance>> map = x => x.Data.Select(b => new Balance()
            {
                Asset = b.CommonAsset,
                Available = b.Available,
                SourceObject = b,
                Total = b.Total
            });
            return WebCallResultMappings.Map(balances, map);
        }

        public async Task<WebCallResult<Order>> GetOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
        {
            long id = 0;
            if (long.TryParse(orderId, out id))
            {
                var order = await GetOrderAsync(id);
                Func<WebCallResult<KunaPlacedOrder>, Order> map = x => new Order()
                {
                    Id = x.Data.Id.ToString(),
                    Price = x.Data.Price,
                    Quantity = x.Data.AmountPlaced,
                    QuantityFilled = x.Data.AmountExecuted,
                    Side = x.Data.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                    SourceObject = x.Data,
                    Status = x.Data.Status switch
                    {

                        KunaOrderStatus.Active => CommonOrderStatus.Active,
                        KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
                        KunaOrderStatus.Filled => CommonOrderStatus.Filled,
                        KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
                        _ => CommonOrderStatus.Canceled

                    },
                    Symbol = x.Data.Symbol,
                    Timestamp = x.Data.TimestampCreated,
                    Type = x.Data.Type switch
                    {
                        KunaOrderType.Limit => CommonOrderType.Limit,
                        KunaOrderType.Market => CommonOrderType.Market,
                        KunaOrderType.MarketByQuote => CommonOrderType.Market,
                        KunaOrderType.StopLimit => CommonOrderType.Other,
                        _ => CommonOrderType.Other
                    },

                };
                return WebCallResultMappings.Map(order, map);
            }
            return new WebCallResult<Order>(new ServerError($"Can not parse id {orderId}"));

        }

        public async Task<WebCallResult<IEnumerable<UserTrade>>> GetOrderTradesAsync(string orderId, string symbol = null, CancellationToken ct = default)
        {
            long id = 0;
            if (long.TryParse(orderId, out id))
            {
                var orderTrades = await GetOrderTradesAsync(symbol, id);
                Func<WebCallResult<List<KunaTrade>>, IEnumerable<UserTrade>> map = x => x.Data.Select(c => new UserTrade()
                {
                    Fee = c.Fee,
                    Id = c.Id.ToString(),
                    OrderId = c.OrderId.ToString(),
                    Price = c.ExecutedPrice,
                    Quantity = c.ExecutedAmount,
                    SourceObject = c,
                    Symbol = c.Pair,
                    Timestamp = c.CommonTradeTime
                });
            }
            return new WebCallResult<IEnumerable<UserTrade>>(new ServerError($"Can not parse id {orderId}"));
        }

        public async Task<WebCallResult<IEnumerable<Order>>> GetOpenOrdersAsync(string symbol = null, CancellationToken ct = default)
        {
            var result = await GetActiveOrdersAsync(symbol);
            Func<WebCallResult<List<KunaPlacedOrder>>, IEnumerable<Order>> map = t => t.Data.Select(x => new Order()
            {
                SourceObject = x,
                Id = x.Id.ToString(),
                Price = x.Price,
                Quantity = x.AmountPlaced,
                QuantityFilled = x.AmountExecuted,
                Side = x.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Status = x.Status switch
                {
                    KunaOrderStatus.Active => CommonOrderStatus.Active,
                    KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
                    KunaOrderStatus.Filled => CommonOrderStatus.Filled,
                    KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
                    _ => CommonOrderStatus.Canceled
                },
                Symbol = x.Symbol,
                Type = x.Type switch
                {
                    KunaOrderType.Limit => CommonOrderType.Limit,
                    KunaOrderType.Market => CommonOrderType.Market,
                    KunaOrderType.MarketByQuote => CommonOrderType.Market,
                    KunaOrderType.StopLimit => CommonOrderType.Other,
                    _ => CommonOrderType.Other
                },
                Timestamp = x.TimestampCreated
            }).ToList();
            return WebCallResultMappings.Map(result, map);
        }

        public async Task<WebCallResult<IEnumerable<Order>>> GetClosedOrdersAsync(string symbol = null, CancellationToken ct = default)
        {
            var result = await GetClosedOrdersAsync(symbol, null, null, 1000, true);
            Func<WebCallResult<List<KunaPlacedOrder>>, IEnumerable<Order>> map = t => t.Data.Select(x => new Order()
            {
                SourceObject = x,
                Id = x.Id.ToString(),
                Price = x.Price,
                Quantity = x.AmountPlaced,
                QuantityFilled = x.AmountExecuted,
                Side = x.OrderSide == KunaOrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Status = x.Status switch
                {
                    KunaOrderStatus.Active => CommonOrderStatus.Active,
                    KunaOrderStatus.Canceled => CommonOrderStatus.Canceled,
                    KunaOrderStatus.Filled => CommonOrderStatus.Filled,
                    KunaOrderStatus.Undefined => CommonOrderStatus.Canceled,
                    _ => CommonOrderStatus.Canceled
                }
            }).ToList();
            return WebCallResultMappings.Map(result, map);
        }

        public async Task<WebCallResult<OrderId>> CancelOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
        {
            long id = 0;
            if (long.TryParse(orderId, out id))
            {
                var cancel = await CancelOrderAsync(id);
                Func<WebCallResult<KunaCanceledOrder>, OrderId> map = x => new OrderId() { SourceObject = x, Id = x.Data.Id.ToString() };
                return WebCallResultMappings.Map(cancel, map);
            }
            return new WebCallResult<OrderId>(new ServerError($"Can not parse id {orderId}"));
        }
        public async Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() {
                { "symbol", symbol }, { "resolution", resolution },
                { "from", JsonConvert.SerializeObject(from, new DateTimeConverter()) },
                { "to", JsonConvert.SerializeObject(to, new DateTimeConverter()) } };
            var result = await SendRequestAsync<TradingViewOhclvV2>(GetUrl(CandlesHistoryEndpoint), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
            if (result)
            {
                List<KunaOhclvV2> data = null;
                if (result.Success)
                {
                    data = new List<KunaOhclvV2>();
                    var t = result.Data;
                    for (int i = 0; i < result.Data.Closes.Count; i++)
                    {
                        var candle = new KunaOhclvV2(t.Timestamps[i], t.Opens[i], t.Highs[i], t.Lows[i], t.Closes[i], t.Volumes[i]);
                        data.Add(candle);
                    }
                }
                return new CallResult<List<KunaOhclvV2>>(data);
            }
            return new CallResult<List<KunaOhclvV2>>(result.Error);

        }
    }
}
