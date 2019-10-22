using CryptoExchange.Net;
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


        #endregion
        public CallResult<DateTime> GetServerTime() => GetServerTimeAsync().Result;
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await ExecuteRequest<string>(GetUrl(ServerTimeEndpoint), "GET").ConfigureAwait(false);
            long seconds = long.Parse(result.Data);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
            return new CallResult<DateTime>(dateTime, null);
        }
        public CallResult<KunaTickerInfo> GetMarketInfo(string market) => GetMarketInfoAsync(market).Result;
        public async Task<CallResult<KunaTickerInfo>> GetMarketInfoAsync(string market)
        {
            var result = await ExecuteRequest<KunaTickerInfo>(GetUrl(FillPathParameter(MarketInfoEndpoint, market)), "GET").ConfigureAwait(false);
            return new CallResult<KunaTickerInfo>(result.Data, result.Error);
        }
        public CallResult<KunaOrderBook> GetOrderBook(string market) => GetOrderBookAsync(market).Result;
        public async Task<CallResult<KunaOrderBook>> GetOrderBookAsync(string market)
        {
            var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = await ExecuteRequest<KunaOrderBook>(GetUrl(OrderBookEndpoint), "GET", parameters).ConfigureAwait(false);
            return new CallResult<KunaOrderBook>(result.Data, result.Error);
        }
        public CallResult<List<KunaTrade>> GetTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetTradesAsync(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTrade>>> GetTradesAsync(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc")
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

            var result = await ExecuteRequest<List<KunaTrade>>(GetUrl(AllTradesEndpoint), "GET", parameters).ConfigureAwait(false);
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }
        public CallResult<KunaAccountInfo> GetAccountInfo() => GetAccountInfoAsync().Result;

        public async Task<CallResult<KunaAccountInfo>> GetAccountInfoAsync()
        {
            var result = await ExecuteRequest<KunaAccountInfo>(GetUrl(AccountInfoEndpoint), "GET", null, true).ConfigureAwait(false);
            return new CallResult<KunaAccountInfo>(result.Data, result.Error);
        }

        public CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market) => PlaceOrderAsync(type, side, volume, price, market).Result;

        public async Task<CallResult<KunaPlacedOrder>> PlaceOrderAsync(OrderType type, OrderSide side, decimal volume, decimal price, string market)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "side", JsonConvert.SerializeObject(side,new OrderSideConverter()) },
                { "type", JsonConvert.SerializeObject(type,new OrderTypeConverter()) },
                { "volume", volume.ToString(CultureInfo.GetCultureInfo("en-US")) },
                { "market", market },
                { "price", price.ToString(CultureInfo.GetCultureInfo("en-US")) }
            };

            var result = await ExecuteRequest<KunaPlacedOrder>(GetUrl(OrdersEndpoint), "POST", parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrder> CancelOrder(long orderId) => CancelOrderAsync(orderId).Result;

        public async Task<CallResult<KunaPlacedOrder>> CancelOrderAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>() { { "id", orderId } };
            var result = await ExecuteRequest<KunaPlacedOrder>(GetUrl(CancelOrderEndpoint), "POST", parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc") => GetMyOrdersAsync(market, orderState, page, sort).Result;

        public async Task<CallResult<List<KunaPlacedOrder>>> GetMyOrdersAsync(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc")
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "state", JsonConvert.SerializeObject(orderState, new OrderStatusConverter())},
                { "order_by", sort },
                { "page", page }
            };

            var result = await ExecuteRequest<List<KunaPlacedOrder>>(GetUrl(OrdersEndpoint), "GET", parameters, true).ConfigureAwait(false);
            return new CallResult<List<KunaPlacedOrder>>(result.Data, result.Error);
        }
        public CallResult<KunaPlacedOrder> GetOrderInfo(long orderId) => GetOrderInfoAsync(orderId).Result;

        public async Task<CallResult<KunaPlacedOrder>> GetOrderInfoAsync(long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
            };

            var result = await ExecuteRequest<KunaPlacedOrder>(GetUrl(SingleOrderEndpoint), "GET", parameters, true).ConfigureAwait(false);
            return new CallResult<KunaPlacedOrder>(result.Data, result.Error);
        }
        public CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc") => GetMyTradesAsync(market, toDate, fromId, toId, limit, sort).Result;

        public async Task<CallResult<List<KunaTrade>>> GetMyTradesAsync(string market, DateTime? toDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "desc")
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
            var result = await ExecuteRequest<List<KunaTrade>>(GetUrl(MyTradesEndpoint), "GET", parameters, true).ConfigureAwait(false);
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }

        public async Task<CallResult<List<KunaOhclv>>> GetCandlesHistoryAsync(string symbol, int resolution, DateTime from, DateTime to)
        {            
            var parameters = new Dictionary<string, object>() { { "symbol", symbol }, { "resolution", resolution }, { "from", JsonConvert.SerializeObject(from, new TimestampSecondsConverter()) }, { "to", JsonConvert.SerializeObject(to, new TimestampSecondsConverter()) } };
            var result = await ExecuteRequest<TradingViewOhclv>(GetUrl(CandlesHistoryEndpoint, "3"), "GET", parameters, false).ConfigureAwait(false);
            List<KunaOhclv> data = null;
            if (result.Success)
            {
                data = new List<KunaOhclv>();
                var t = result.Data;
                for(int i=0; i < result.Data.Closes.Count; i++)
                {
                    var candle = new KunaOhclv(t.Timestamps[i], t.Opens[i], t.Highs[i], t.Lows[i],t.Closes[i],t.Volumes[i]);
                    data.Add(candle);
                }
            }
            return new CallResult<List<KunaOhclv>>(data, result.Error);
        }

        #region BaseMethodOverride
        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            var uriString = uri.ToString();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(new Uri(uriString).PathAndQuery, method, parameters, signed);
            if ((method == Constants.GetMethod || method == Constants.DeleteMethod || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
            {
                uriString += "?" + parameters.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
            }

            var request = RequestFactory.Create(uriString);
            request.ContentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            request.Accept = Constants.JsonContentHeader;
            request.Method = method;
            //var headers = new Dictionary<string, string>();


            if ((method == Constants.PostMethod || method == Constants.PutMethod) && postParametersPosition != PostParameters.InUri)
            {
                if (parameters?.Any() == true)
                    WriteParamBody(request, JsonConvert.SerializeObject(parameters));
                else
                    WriteParamBody(request, "{}");
            }

            return request;
        }

        protected Uri GetUrl(string endpoint, string version = null)
        {
            return version == null ? new Uri($"{BaseAddress}/{endpoint}") : new Uri($"https://api.kuna.io/v{version}/{endpoint}");
        }
        public CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo() => GetExchangeCurrenciesInfoAsync().Result;

        public async Task<CallResult<List<KunaTraidingPair>>> GetExchangeCurrenciesInfoAsync()
        {
            string url = "https://api.kuna.io/v3/markets";
            var result = await ExecuteRequest<List<KunaTraidingPair>>(new Uri(url), "GET", null, false).ConfigureAwait(false);
            return new CallResult<List<KunaTraidingPair>>(result.Data, result.Error);
        }

        #endregion

    }
}
