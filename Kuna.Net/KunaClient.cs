using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Kuna.Net.Converters;
using Kuna.Net.Helpers;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaClient : RestClient, IKunaClientV2, IKunaClientV3
    {
        private readonly bool IsProAccount;
        public KunaClient() : base("KunaApiClient", new KunaClientOptions(), null)
        {

        }
        public KunaClient(KunaClientOptions options, string clientName = "KunaApiClient") : base(clientName, options, options.ApiCredentials == null ? null : new KunaAuthenticationProvider(options.ApiCredentials))
        {
            IsProAccount = options.IsProAccount;
            postParametersPosition = PostParameters.InUri;
            requestBodyFormat = RequestBodyFormat.Json;
        }
        #region Endpoints
        private const string LatestVersion = "3";
        private const string ServerTimeEndpoint = "timestamp";
        private const string MarketInfoV2Endpoint = "tickers/{}";
        private const string OrderBookV2Endpoint = "depth";
        private const string AllTradesEndpoint = "trades";
        private const string AccountInfoEndpoint = "members/me";
        private const string OrdersV2Endpoint = "orders";
        private const string SingleOrderV2Endpoint = "order";
        private const string CancelOrderEndpoint = "order/delete";
        private const string MyTradesEndpoint = "trades/my";
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
        public CallResult<DateTime> GetServerTimeV2() => GetServerTimeV2Async().Result;
        public async Task<CallResult<DateTime>> GetServerTimeV2Async(CancellationToken ct = default)
        {
            var result = await SendRequest<string>(GetUrl(ServerTimeEndpoint), HttpMethod.Get, ct, null, false, false).ConfigureAwait(false);
            long seconds = long.Parse(result.Data);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return new CallResult<DateTime>(dateTime, null);
        }
        public CallResult<KunaTickerInfoV2> GetMarketInfoV2(string market) => GetMarketInfoV2Async(market).Result;
        public async Task<CallResult<KunaTickerInfoV2>> GetMarketInfoV2Async(string market, CancellationToken ct = default)
        {
            var result = await SendRequest<KunaTickerInfoV2>(GetUrl(FillPathParameter(MarketInfoV2Endpoint, market)), HttpMethod.Get, ct, null, false, false).ConfigureAwait(false);
            return new CallResult<KunaTickerInfoV2>(result.Data, result.Error);
        }
        public CallResult<KunaOrderBookV2> GetOrderBookV2(string market, int limit = 1000) => GetOrderBookV2Async(market, limit).Result;
        public async Task<CallResult<KunaOrderBookV2>> GetOrderBookV2Async(string market, int limit = 1000, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "limit", limit } };
            var result = await SendRequest<KunaOrderBookV2>(GetUrl(OrderBookV2Endpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            return new CallResult<KunaOrderBookV2>(result.Data, result.Error);
        }
        public CallResult<List<KunaTradeV2>> GetTradesV2(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetTradesV2Async(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTradeV2>>> GetTradesV2Async(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "order_by", sort } };
            if (toDate != null)
            {
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new TimestampSecondsConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);

            var result = await SendRequest<List<KunaTradeV2>>(GetUrl(AllTradesEndpoint), HttpMethod.Get, ct, parameters, false, false).ConfigureAwait(false);
            return new CallResult<List<KunaTradeV2>>(result.Data, result.Error);
        }
        public CallResult<KunaAccountInfoV2> GetAccountInfoV2() => GetAccountInfoV2Async().Result;

        public async Task<CallResult<KunaAccountInfoV2>> GetAccountInfoV2Async(CancellationToken ct = default)
        {
            var result = await SendRequest<KunaAccountInfoV2>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true, false).ConfigureAwait(false);
            return new CallResult<KunaAccountInfoV2>(result.Data, result.Error);
        }

        public CallResult<KunaPlacedOrderV2> PlaceOrderV2(KunaOrderTypeV2 type, KunaOrderSideV2 side, decimal volume, decimal price, string market) => PlaceOrderV2Async(type, side, volume, price, market).Result;

        public async Task<CallResult<KunaPlacedOrderV2>> PlaceOrderV2Async(KunaOrderTypeV2 type, KunaOrderSideV2 side, decimal volume, decimal price, string market, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "side", JsonConvert.SerializeObject(side,new OrderSideConverter()) },
                { "type", JsonConvert.SerializeObject(type,new OrderTypeV2Converter()) },
                { "volume", volume.ToString(CultureInfo.GetCultureInfo("en-US")) },
                { "market", market },
                { "price", price.ToString(CultureInfo.GetCultureInfo("en-US")) }
            };

            var result = await SendRequest<KunaPlacedOrderV2>(GetUrl(OrdersV2Endpoint), HttpMethod.Post, ct, parameters, true, false).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrderV2>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrderV2> CancelOrderV2(long orderId) => CancelOrderV2Async(orderId).Result;

        public async Task<CallResult<KunaPlacedOrderV2>> CancelOrderV2Async(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "id", orderId } };
            var result = await SendRequest<KunaPlacedOrderV2>(GetUrl(CancelOrderEndpoint), HttpMethod.Post, ct, parameters, true, false).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrderV2>(result.Data, result.Error);
        }
        public CallResult<List<KunaPlacedOrderV2>> GetMyOrdersV2(string market, KunaOrderStateV2 orderState = KunaOrderStateV2.Wait, int page = 1, string sort = "desc") => GetMyOrdersV2Async(market, orderState, page, sort).Result;

        public async Task<CallResult<List<KunaPlacedOrderV2>>> GetMyOrdersV2Async(string market, KunaOrderStateV2 orderState = KunaOrderStateV2.Wait, int page = 1, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "state", JsonConvert.SerializeObject(orderState, new OrderStatusV2Converter())},
                { "order_by", sort },
                { "page", page }
            };

            var result = await SendRequest<List<KunaPlacedOrderV2>>(GetUrl(OrdersV2Endpoint), HttpMethod.Get, ct, parameters, true, false).ConfigureAwait(false);
            return new CallResult<List<KunaPlacedOrderV2>>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrderV2> GetOrderInfoV2(long orderId) => GetOrderInfoV2Async(orderId).Result;

        public async Task<CallResult<KunaPlacedOrderV2>> GetOrderInfoV2Async(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
            };

            var result = await SendRequest<KunaPlacedOrderV2>(GetUrl(SingleOrderV2Endpoint), HttpMethod.Get, ct, parameters, true, false).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrderV2>(result.Data, result.Error);
        }
        public CallResult<List<KunaTradeV2>> GetMyTradesV2(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetMyTradesV2Async(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTradeV2>>> GetMyTradesV2Async(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "order_by", sort }, };
            if (toDate != null)
            {
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new TimestampSecondsConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);
            var result = await SendRequest<List<KunaTradeV2>>(GetUrl(MyTradesEndpoint), HttpMethod.Get, ct, parameters, true, false).ConfigureAwait(false);
            return new CallResult<List<KunaTradeV2>>(result.Data, result.Error);
        }
        public async Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "symbol", symbol }, { "resolution", resolution }, { "from", JsonConvert.SerializeObject(from, new TimestampSecondsConverter()) }, { "to", JsonConvert.SerializeObject(to, new TimestampSecondsConverter()) } };
            var result = await SendRequest<TradingViewOhclvV2>(GetUrl(CandlesHistoryEndpoint, "3"), HttpMethod.Get, ct, parameters, false, false).ConfigureAwait(false);
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
            return new CallResult<List<KunaOhclvV2>>(data, result.Error);
        }

        #region BaseMethodOverride


        protected override IRequest ConstructRequest(Uri uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, PostParameters postParameterPosition, ArrayParametersSerialization arraySerialization, int requestId)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            var uriString = uri.ToString();

            if (uriString.Contains("v2"))
            {
                if (authProvider != null && signed)
                    parameters = authProvider.AddAuthenticationToParameters(new Uri(uriString).PathAndQuery, method, parameters, signed, postParametersPosition, arraySerialization);

            }
            if ((method == HttpMethod.Get || method == HttpMethod.Delete || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
            {
                uriString += "?" + parameters.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
            }
            var request = RequestFactory.Create(method, uriString, requestId);
            request.Accept = Constants.JsonContentHeader;
            request.Method = method;
            if (uriString.Contains("v3"))
            {
                if (authProvider != null)
                {
                    var headers = authProvider.AddAuthenticationToHeaders(uriString, method, parameters, signed, postParametersPosition, arraySerialization);
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
            }
            if ((method == HttpMethod.Post || method == HttpMethod.Put) && postParametersPosition != PostParameters.InUri)
            {
                WriteParamBody(request, parameters, requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader);
            }

            return request;
        }

        protected Uri GetUrl(string endpoint, string version = null)
        {
            if (version != null)
            {
                postParametersPosition = PostParameters.InBody;
            }
            else
            {
                postParametersPosition = PostParameters.InUri;
            }
            return version == null ? new Uri($"{BaseAddress}{endpoint}") : new Uri($"https://api.kuna.io/v{version}/{endpoint}");

        }

        public CallResult<List<KunaOhclvV2>> GetCandlesHistoryV2(string symbol, int resolution, DateTime from, DateTime to) => GetCandlesHistoryV2Async(symbol, resolution, from, to).Result;

        #endregion

        #region implementing IKunaClientV3
        public WebCallResult<IEnumerable<KunaTicker>> GetTickers(params string[] symbols) => GetTickersAsync(default, symbols).Result;
        public async Task<WebCallResult<IEnumerable<KunaTicker>>> GetTickersAsync(CancellationToken ct = default, params string[] symbols)
        {
            var request = new Dictionary<string, object>();
            string symb = symbols.AsStringParameterOrNull() ?? "ALL";
            request.AddOptionalParameter("symbols", symb);
            return await SendRequest<IEnumerable<KunaTicker>>(GetUrl(TickersEndpoint, "3"), HttpMethod.Get, ct, request, false, false);

        }
        public WebCallResult<KunaOrderBook> GetOrderBook(string symbol) => GetOrderBookAsync(symbol).Result;
        public async Task<WebCallResult<KunaOrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProBookEnpoint : OrderBookEndpoint;
            var result = await SendRequest<IEnumerable<KunaOrderBookEntry>>(GetUrl(FillPathParameter(url, symbol), "3"), IsProAccount ? HttpMethod.Post : HttpMethod.Get, ct, null, IsProAccount).ConfigureAwait(false);
            return new WebCallResult<KunaOrderBook>(result.ResponseStatusCode, result.ResponseHeaders, result ? new KunaOrderBook(result.Data) : null, result.Error);
        }

        public WebCallResult<DateTime?> GetServerTime() => GetServerTimeAsync().Result;
        public async Task<WebCallResult<DateTime?>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await SendRequest<KunaTimestampResponse>(GetUrl(ServerTimeEndpoint, "3"), HttpMethod.Get, ct, null, false, false).ConfigureAwait(false);
            return new WebCallResult<DateTime?>(result.ResponseStatusCode, result.ResponseHeaders, result.Data?.CurrentTime, result.Error);
        }

        public WebCallResult<IEnumerable<KunaTradingPair>> GetTradingPairs() => GetTradingPairsAsync().Result;

        public async Task<WebCallResult<IEnumerable<KunaTradingPair>>> GetTradingPairsAsync(CancellationToken ct = default)
        {
            return await SendRequest<IEnumerable<KunaTradingPair>>(GetUrl(TradingPairsEndpoint, "3"), HttpMethod.Get, ct, null, false, false).ConfigureAwait(false);
        }

        public WebCallResult<List<KunaCurrency>> GetCurrencies(bool? privileged = null) => GetCurrenciesAsync(privileged, default).Result;
        public async Task<WebCallResult<List<KunaCurrency>>> GetCurrenciesAsync(bool? privileged = null, CancellationToken ct = default)
        {
            var request = new Dictionary<string, object>();
            request.AddOptionalParameter("privileged", privileged);
            return await SendRequest<List<KunaCurrency>>(GetUrl(CurrenciesEndpoint, "3"), HttpMethod.Get, ct, request, false, false);
        }
        public WebCallResult<IEnumerable<KunaPublicTrade>> GetRecentPublicTrades(string symbol, int limit = 25) => GetRecentPublicTradesAsync(symbol, limit).Result;

        public async Task<WebCallResult<IEnumerable<KunaPublicTrade>>> GetRecentPublicTradesAsync(string symbol, int limit = 25, CancellationToken ct = default)
        {
            limit = limit > 500 ? 500 : limit;
            var parameters = new Dictionary<string, object>();
            parameters.AddParameter("limit", limit);
            return await SendRequest<IEnumerable<KunaPublicTrade>>(GetUrl(FillPathParameter(PublicTradesEndPoint, symbol), "3"), HttpMethod.Get, ct, parameters, false, false);
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
            var request = await SendRequest<KunaPlacedOrder>(GetUrl(url, "3"), HttpMethod.Post, ct, parameters, true, false, PostParameters.InBody);
            return request;
        }

        public WebCallResult<KunaPlacedOrder> PlaceOrder(string symbol, KunaOrderSide side, KunaOrderType orderType, decimal quantity, decimal? price = null, decimal? stopPrice = null) => PlaceOrderAsync(symbol, side, orderType, quantity, price, stopPrice).Result;

        public async Task<WebCallResult<KunaCanceledOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProCancelOrderEndpoint : V3CancelOrderEndpoint;
            return await SendRequest<KunaCanceledOrder>(GetUrl(url, "3"), HttpMethod.Post, ct, new Dictionary<string, object>() { { "order_id", orderId } }, true, false, PostParameters.InBody);
        }

        public WebCallResult<KunaCanceledOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;


        public async Task<WebCallResult<List<KunaPlacedOrder>>> CancelOrdersAsync(List<long> orderIds, CancellationToken ct = default)
        {
            string url = IsProAccount ? ProCancelMultipleOrdersEndpoint : V3CancelOrderEndpoint + "/multi";
            return await SendRequest<List<KunaPlacedOrder>>(GetUrl(url, "3"), HttpMethod.Post, ct, new Dictionary<string, object>() { { "order_ids", JsonConvert.SerializeObject(orderIds) } }, true, false, PostParameters.InBody);

        }
        public WebCallResult<List<KunaPlacedOrder>> CancelOrders(List<long> orderIds) => CancelOrdersAsync(orderIds).Result;

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
            var url = GetUrl(endpoint, "3");

            var parameters = new Dictionary<string, object>();
            if (from.HasValue)
                parameters.AddOptionalParameter("start", JsonConvert.SerializeObject(from, new TimestampConverter()));
            if (to.HasValue)
                parameters.AddOptionalParameter("end", JsonConvert.SerializeObject(to, new TimestampConverter()));
            if (limit.HasValue)
                parameters.AddOptionalParameter("limit", limit.Value);
            if (sortDesc.HasValue)
                parameters.AddOptionalParameter("sort", sortDesc.Value ? -1 : 1);
            var result = await SendRequest<List<KunaPlacedOrder>>(url, HttpMethod.Post, ct, parameters, true, false);
            return result;
        }
        public WebCallResult<List<KunaPlacedOrder>> GetOrders(KunaOrderStatus state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null)
        => GetOrdersAsync(state, market, from, to, limit, sortDesc).Result;

        public async Task<WebCallResult<List<KunaPlacedOrder>>> GetActiveOrdersAsync(string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default)
        => await GetOrdersAsync(KunaOrderStatus.Active, market, from, to, limit,sortDesc, ct);

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
            return new WebCallResult<List<KunaPlacedOrder>>(sCode, headers, orders, error);
        }
        public WebCallResult<KunaPlacedOrder> GetOrder(long id) => GetOrderAsync(id).Result;

        public async Task<WebCallResult<KunaPlacedOrder>> GetOrderAsync(long id, CancellationToken ct = default)
        {
            var url = IsProAccount ? ProOrderDetailsEndpoint : OrderDetailsEndpoint;
            var parameters = new Dictionary<string, object>();
            parameters.AddParameter("id", id);
            var result = await SendRequest<KunaPlacedOrder>(GetUrl(url, "3"), HttpMethod.Post, ct, parameters, true, false);
            return result;
        }

        public WebCallResult<List<KunaTrade>> GetOrderTrades(string market, long id) => GetOrderTradesAsync(market, id).Result;
        public async Task<WebCallResult<List<KunaTrade>>> GetOrderTradesAsync(string market, long id, CancellationToken ct = default)
        {
            var url = GetUrl($"auth/r/order/{market}:{id}/trades", "3");
            var result = await SendRequest<List<KunaTrade>>(url, HttpMethod.Post, ct, new Dictionary<string, object>(), true, false);
            return result;
        }
        public WebCallResult<IEnumerable<KunaAccountBalance>> GetBalances() => GetBalancesAsync().Result;
        public async Task<WebCallResult<IEnumerable<KunaAccountBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            string url = IsProAccount ? ProWalletsEndpoint : WalletEndpoint;
            return await SendRequest<IEnumerable<KunaAccountBalance>>(GetUrl(url, "3"), HttpMethod.Post, ct, null, true, false);
        }

        #endregion implementing IKunaClientV3

        #region implementing IExchangeClient
        public string GetSymbolName(string baseAsset, string quoteAsset)
        {
            return baseAsset + quoteAsset;
        }

        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> IExchangeClient.GetSymbolsAsync()
        {
            var result = await GetTradingPairsAsync();
            return WebCallResult<IEnumerable<ICommonSymbol>>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var result = await GetTickersAsync(default);
            return WebCallResult<IEnumerable<ICommonTicker>>.CreateFrom(result);
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var result = await GetTickersAsync(symbols: symbol);
            var singleTickerResult = new WebCallResult<KunaTicker>(
                result.ResponseStatusCode,
                result.ResponseHeaders,
                result ? result.Data.FirstOrDefault() : null,
                result.Error);
            return WebCallResult<ICommonTicker>.CreateFrom(singleTickerResult);
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var result = await GetOrderBookAsync(symbol, default);
            return WebCallResult<ICommonOrderBook>.CreateFrom(result);
        }

        public async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> GetRecentTradesAsync(string symbol)
        {
            var result = await GetRecentPublicTradesAsync(symbol);
            return WebCallResult<IEnumerable<ICommonRecentTrade>>.CreateFrom(result);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string accountId = null)
        {
            var kunaSide = side == IExchangeClient.OrderSide.Buy ? KunaOrderSide.Buy : KunaOrderSide.Sell;
            var kunaType = type switch
            {
                IExchangeClient.OrderType.Limit => KunaOrderType.Limit,
                IExchangeClient.OrderType.Market => KunaOrderType.Market,
                _ => throw new NotImplementedException("Undefined order type. Use market or limit")
            };
            var result = await PlaceOrderAsync(symbol, kunaSide, kunaType, quantity, price);
            return WebCallResult<ICommonOrderId>.CreateFrom(result);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string symbol = null)
        {
            var result = await GetOrderAsync(ParseToLongOrderId(orderId));
            return WebCallResult<ICommonOrder>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string symbol)
        {
            var result = await GetOrderTradesAsync(symbol, ParseToLongOrderId(orderId));
            return WebCallResult<IEnumerable<ICommonTrade>>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string symbol = null)
        {
            var result = await GetOrdersAsync(KunaOrderStatus.Active, symbol);
            return WebCallResult<IEnumerable<ICommonOrder>>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string symbol = null)
        {
            var result = await GetOrdersAsync(KunaOrderStatus.Filled, symbol);
            return WebCallResult<IEnumerable<ICommonOrder>>.CreateFrom(result);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string symbol = null)
        {
            var result = await CancelOrderAsync(ParseToLongOrderId(orderId));
            return WebCallResult<ICommonOrderId>.CreateFrom(result);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string accountId = null)
        {
            var result = await GetBalancesAsync();
            return WebCallResult<IEnumerable<ICommonBalance>>.CreateFrom(result);
        }

        #endregion implementing IExchangeClient

        private long ParseToLongOrderId(string orderId)
        {
            long id;
            if (long.TryParse(orderId, out id))
            {
                return id;
            }
            else
            {
                throw new ArgumentException("Can't convert \"orderId\" to type long");
            }
        }

        public async Task<CallResult> GetTradesHistoryToEmail(string symbol, CancellationToken ct = default)
        {
            var request = await SendRequest<object>(GetUrl("auth/history/trades", "3"), HttpMethod.Post, default, new Dictionary<string, object>() { { "market", symbol } }, true);
            return request;
        }
        
    }
}
