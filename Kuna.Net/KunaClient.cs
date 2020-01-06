﻿using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;
using CryptoExchange.Net.Requests;
using Kuna.Net.Converters;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
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
    public class KunaClient : RestClient, IKunaClient
    {
        public KunaClient() : base(new KunaClientOptions(), null)
        {

        }
        public KunaClient(KunaClientOptions options) : base(options, options.ApiCredentials == null ? null : new KunaAuthenticationProvider(options.ApiCredentials))
        {
            postParametersPosition = PostParameters.InUri;
            requestBodyFormat = RequestBodyFormat.Json;
        }
        #region Endpoints
        private const string ServerTimeEndpoint = "timestamp";
        private const string MarketInfoEndpoint = "tickers/{}";
        private const string OrderBookEndpoint = "depth";
        private const string AllTradesEndpoint = "trades";
        private const string AccountInfoEndpoint = "members/me";
        private const string OrdersEndpoint = "orders";
        private const string SingleOrderEndpoint = "order";

        private const string CancelOrderEndpoint = "order/delete";
        private const string MyTradesEndpoint = "trades/my";
        private const string CandlesHistoryEndpoint = "tv/history";
        private const string Orders3 = "auth/r/orders/";

        #endregion
        public CallResult<DateTime> GetServerTime() => GetServerTimeAsync().Result;
        public async Task<CallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await SendRequest<string>(GetUrl(ServerTimeEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
            long seconds = long.Parse(result.Data);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return new CallResult<DateTime>(dateTime, null);
        }
        public CallResult<KunaTickerInfo> GetMarketInfo(string market) => GetMarketInfoAsync(market).Result;
        public async Task<CallResult<KunaTickerInfo>> GetMarketInfoAsync(string market, CancellationToken ct = default)
        {
            var result = await SendRequest<KunaTickerInfo>(GetUrl(FillPathParameter(MarketInfoEndpoint, market)), HttpMethod.Get, ct).ConfigureAwait(false);
            return new CallResult<KunaTickerInfo>(result.Data, result.Error);
        }
        public CallResult<KunaOrderBook> GetOrderBook(string market) => GetOrderBookAsync(market).Result;
        public async Task<CallResult<KunaOrderBook>> GetOrderBookAsync(string market, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = await SendRequest<KunaOrderBook>(GetUrl(OrderBookEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            return new CallResult<KunaOrderBook>(result.Data, result.Error);
        }
        public CallResult<List<KunaTrade>> GetTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetTradesAsync(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTrade>>> GetTradesAsync(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
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

            var result = await SendRequest<List<KunaTrade>>(GetUrl(AllTradesEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }
        public CallResult<KunaAccountInfo> GetAccountInfo() => GetAccountInfoAsync().Result;

        public async Task<CallResult<KunaAccountInfo>> GetAccountInfoAsync(CancellationToken ct = default)
        {
            var result = await SendRequest<KunaAccountInfo>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
            return new CallResult<KunaAccountInfo>(result.Data, result.Error);
        }
        public CallResult<List<KunaPlacedOrderV3>> GetOrders3(OrderState state, string market = null, DateTime? from = null, DateTime? to = null, int limit = 100, bool sortDesc = false, CancellationToken ct=default)
        {
            var endpoint = Orders3;
            if (!String.IsNullOrEmpty(market))
            {
                endpoint += $"{market}";
            }
            if (state == OrderState.Done || state == OrderState.Cancel)
            {
                endpoint += "/hist";
            }
            var url = GetUrl(endpoint, "3");

            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("start", JsonConvert.SerializeObject(from, new TimestampConverter()));
            parameters.AddOptionalParameter("end", JsonConvert.SerializeObject(to, new TimestampConverter()));
            parameters.AddOptionalParameter("limit", limit);
            parameters.AddOptionalParameter("sort", sortDesc ? -1 : 1);
            var result = SendRequest<List<KunaPlacedOrderV3>>(url, HttpMethod.Post,ct, parameters, true).Result;
            return result;
        }

        public CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market) => PlaceOrderAsync(type, side, volume, price, market).Result;

        public async Task<CallResult<KunaPlacedOrder>> PlaceOrderAsync(OrderType type, OrderSide side, decimal volume, decimal price, string market, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "side", JsonConvert.SerializeObject(side,new OrderSideConverter()) },
                { "type", JsonConvert.SerializeObject(type,new OrderTypeConverter()) },
                { "volume", volume.ToString(CultureInfo.GetCultureInfo("en-US")) },
                { "market", market },
                { "price", price.ToString(CultureInfo.GetCultureInfo("en-US")) }
            };

            var result = await SendRequest<KunaPlacedOrder>(GetUrl(OrdersEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;

        public async Task<CallResult<KunaPlacedOrder>> CancelOrderAsync(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "id", orderId } };
            var result = await SendRequest<KunaPlacedOrder>(GetUrl(CancelOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc") => GetMyOrdersAsync(market, orderState, page, sort).Result;

        public async Task<CallResult<List<KunaPlacedOrder>>> GetMyOrdersAsync(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "state", JsonConvert.SerializeObject(orderState, new OrderStatusConverter())},
                { "order_by", sort },
                { "page", page }
            };

            var result = await SendRequest<List<KunaPlacedOrder>>(GetUrl(OrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            return new CallResult<List<KunaPlacedOrder>>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrder> GetOrderInfo(long orderId) => GetOrderInfoAsync(orderId).Result;

        public async Task<CallResult<KunaPlacedOrder>> GetOrderInfoAsync(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
            };

            var result = await SendRequest<KunaPlacedOrder>(GetUrl(SingleOrderEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetMyTradesAsync(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTrade>>> GetMyTradesAsync(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
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
            var result = await SendRequest<List<KunaTrade>>(GetUrl(MyTradesEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }

        public async Task<CallResult<List<KunaOhclv>>> GetCandlesHistoryAsync(string symbol, int resolution, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "symbol", symbol }, { "resolution", resolution }, { "from", JsonConvert.SerializeObject(from, new TimestampSecondsConverter()) }, { "to", JsonConvert.SerializeObject(to, new TimestampSecondsConverter()) } };
            var result = await SendRequest<TradingViewOhclv>(GetUrl(CandlesHistoryEndpoint, "3"), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
            List<KunaOhclv> data = null;
            if (result.Success)
            {
                data = new List<KunaOhclv>();
                var t = result.Data;
                for (int i = 0; i < result.Data.Closes.Count; i++)
                {
                    var candle = new KunaOhclv(t.Timestamps[i], t.Opens[i], t.Highs[i], t.Lows[i], t.Closes[i], t.Volumes[i]);
                    data.Add(candle);
                }
            }
            return new CallResult<List<KunaOhclv>>(data, result.Error);
        }

        #region BaseMethodOverride

        protected override IRequest ConstructRequest(Uri uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            var uriString = uri.ToString();
            if(uriString.Contains("v2"))
            {
                if (authProvider != null)
                    parameters = authProvider.AddAuthenticationToParameters(new Uri(uriString).PathAndQuery, method, parameters, signed);
                if ((method == HttpMethod.Get || method == HttpMethod.Delete || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
                {
                    uriString += "?" + parameters.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
                }
            }
           

            var request = RequestFactory.Create(method, uriString);
            // request.Content = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            request.Accept = Constants.JsonContentHeader;
            request.Method = method;
            //var headers = new Dictionary<string, string>();
            if (uriString.Contains("v3"))
            {
                if (authProvider != null)
                {
                    var headers = authProvider.AddAuthenticationToHeaders(uriString, method, parameters, signed);
                    foreach(var header in headers)
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
            return version == null ? new Uri($"{BaseAddress}/{endpoint}") : new Uri($"https://api.kuna.io/v{version}/{endpoint}");
        }
        public CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo() => GetExchangeCurrenciesInfoAsync().Result;

        public async Task<CallResult<List<KunaTraidingPair>>> GetExchangeCurrenciesInfoAsync(CancellationToken ct = default)
        {
            string url = "https://api.kuna.io/v3/markets";
            var result = await SendRequest<List<KunaTraidingPair>>(new Uri(url), HttpMethod.Get, ct, null, false).ConfigureAwait(false);
            return new CallResult<List<KunaTraidingPair>>(result.Data, result.Error);
        }

        public CallResult<List<KunaOhclv>> GetCandlesHistory(string symbol, int resolution, DateTime from, DateTime to) => GetCandlesHistoryAsync(symbol, resolution, from, to).Result;

        #endregion

    }
}
