using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using Kuna.Net.Converters;
using Kuna.Net.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Objects.V2
{
    public class KunaV2ApiClient : RestApiClient, IKunaApiClientV2
    {
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

        private readonly KunaClient _kunaClient;

        public KunaV2ApiClient(BaseRestClientOptions options, RestApiClientOptions apiOptions) : base(options, apiOptions)
        {
        }
        protected override TimeSyncInfo GetTimeSyncInfo()
        {
            throw new NotImplementedException();
        }

        public override TimeSpan GetTimeOffset()
        {
            throw new NotImplementedException();
        }

        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        {
            throw new NotImplementedException();
        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            throw new NotImplementedException();
        }

        protected Uri GetUrl(string endpoint, string version = null)
        {
            return version == null ? new Uri($"{BaseAddress}{endpoint}") : new Uri($"https://api.kuna.io/v{version}/{endpoint}");
        }
        private async Task<WebCallResult<T>> SendRequestAsync<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object> request, bool signed) where T : class
        {
            return await _kunaClient.SendRequestInternal<T>(this, uri, method, ct, request, signed);
        }
        public CallResult<DateTime> GetServerTimeV2() => GetServerTimeV2Async().Result;
        public async Task<CallResult<DateTime>> GetServerTimeV2Async(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<string>(GetUrl(ServerTimeEndpoint), HttpMethod.Get, ct, null, false).ConfigureAwait(false);
            long seconds = long.Parse(result.Data);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return new CallResult<DateTime>(dateTime);
        }
        public CallResult<KunaTickerInfoV2> GetMarketInfoV2(string market) => GetMarketInfoV2Async(market).Result;
        public async Task<CallResult<KunaTickerInfoV2>> GetMarketInfoV2Async(string market, CancellationToken ct = default)
        {
            return await SendRequestAsync<KunaTickerInfoV2>(GetUrl(FillPathParameter(MarketInfoV2Endpoint, market)), HttpMethod.Get, ct, null, false).ConfigureAwait(false);
        }
        public CallResult<KunaOrderBookV2> GetOrderBookV2(string market, int limit = 1000) => GetOrderBookV2Async(market, limit).Result;
        public async Task<CallResult<KunaOrderBookV2>> GetOrderBookV2Async(string market, int limit = 1000, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "limit", limit } };
            return await SendRequestAsync<KunaOrderBookV2>(GetUrl(OrderBookV2Endpoint), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
        }
        public CallResult<List<KunaTradeV2>> GetTradesV2(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetTradesV2Async(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTradeV2>>> GetTradesV2Async(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "order_by", sort } };
            if (toDate != null)
            {
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new DateTimeConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);

            return await SendRequestAsync<List<KunaTradeV2>>(GetUrl(AllTradesEndpoint), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
        }
        public CallResult<KunaAccountInfoV2> GetAccountInfoV2() => GetAccountInfoV2Async().Result;

        public async Task<CallResult<KunaAccountInfoV2>> GetAccountInfoV2Async(CancellationToken ct = default)
        {
            return await SendRequestAsync<KunaAccountInfoV2>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
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

            return await SendRequestAsync<KunaPlacedOrderV2>(GetUrl(OrdersV2Endpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
        public CallResult<KunaPlacedOrderV2> CancelOrderV2(long orderId) => CancelOrderV2Async(orderId).Result;

        public async Task<CallResult<KunaPlacedOrderV2>> CancelOrderV2Async(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "id", orderId } };
            return await SendRequestAsync<KunaPlacedOrderV2>(GetUrl(CancelOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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

            return await SendRequestAsync<List<KunaPlacedOrderV2>>(GetUrl(OrdersV2Endpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);

        }
        public CallResult<KunaPlacedOrderV2> GetOrderInfoV2(long orderId) => GetOrderInfoV2Async(orderId).Result;

        public async Task<CallResult<KunaPlacedOrderV2>> GetOrderInfoV2Async(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
            };

            var result = await SendRequestAsync<KunaPlacedOrderV2>(GetUrl(SingleOrderV2Endpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                return new CallResult<KunaPlacedOrderV2>(result.Data);
            }
            else
            {
                return new CallResult<KunaPlacedOrderV2>(result.Error);
            }
        }
        public CallResult<List<KunaTradeV2>> GetMyTradesV2(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetMyTradesV2Async(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTradeV2>>> GetMyTradesV2Async(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() { { "market", market }, { "order_by", sort }, };
            if (toDate != null)
            {
                parameters.AddOptionalParameter("timestamp", JsonConvert.SerializeObject(toDate, new DateTimeConverter()));
            }
            parameters.AddOptionalParameter("from", fromId);
            parameters.AddOptionalParameter("to", toId);
            if (limit > 1000)
            {
                limit = 1000;
            }
            parameters.AddOptionalParameter("limit", limit);
            var result = await SendRequestAsync<List<KunaTradeV2>>(GetUrl(MyTradesEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                return new CallResult<List<KunaTradeV2>>(result.Data);

            }
            else
            {
                return new CallResult<List<KunaTradeV2>>(result.Error);

            }
        }
        public async Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>() {
                { "symbol", symbol }, { "resolution", resolution },
                { "from", JsonConvert.SerializeObject(from, new DateTimeConverter()) },
                { "to", JsonConvert.SerializeObject(to, new DateTimeConverter()) } };
            var result = await SendRequestAsync<TradingViewOhclvV2>(GetUrl(CandlesHistoryEndpoint, "3"), HttpMethod.Get, ct, parameters, false).ConfigureAwait(false);
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
        public CallResult<List<KunaOhclvV2>> GetCandlesHistoryV2(string symbol, int resolution, DateTime from, DateTime to) => GetCandlesHistoryV2Async(symbol, resolution, from, to).Result;
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

       
    }
}
