using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Requests;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuna.Net
{
    public class KunaClient : RestClient, IKunaClient
    {
        public KunaClient(KunaClientOptions options, AuthenticationProvider authenticationProvider) : base(options, authenticationProvider)
        {
            postParametersPosition = PostParameters.InUri;
            requestBodyFormat = RequestBodyFormat.Json;
        }
        #region Endpoints
        private const string ServerTimeEndpoint = "timestamp";
        private const string MarketInfoEndpoint = "tickers/{}";
        private const string OrderBookEndpoint = "depth";
        private const string AllTradesEndpoint = "trades";
        private const string AccountInfoEndpoint= "members/me";
        private const string OrdersEndpoint = "orders";
        private const string CancelOrderEndpoint = "order/delete";
        private const string MyTradesEndpoint = "trades/my";

        #endregion
        public CallResult<DateTime> GetServerTime()
        {
            //var parameters = new Dictionary<string, object>() { { "product_id", id } };
            //parameters.AddOptionalParameter("limit", limit);
            //parameters.AddOptionalParameter("page", page);
            //if (limit > 1000 || limit < 1)
            //    return new CallResult<LiquidQuoineDefaultResponse<LiquidQuoineExecution>>(null, new ServerError("Limit should be between 1 and 1000"));

            var result =  ExecuteRequest<string>(GetUrl(ServerTimeEndpoint), "GET").Result;
            long seconds = long.Parse(result.Data);
            var dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

            return new CallResult<DateTime>(dateTime,null);
        }

        public CallResult<KunaTickerInfo> GetMarketInfo(string market)
        {
         //   var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = ExecuteRequest<KunaTickerInfo>(GetUrl(FillPathParameter(MarketInfoEndpoint,market)), "GET").Result;
            return new CallResult<KunaTickerInfo>(result.Data, result.Error);

        }

        public CallResult<KunaOrderBook> GetOrderBook(string market)
        {
            var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = ExecuteRequest<KunaOrderBook>(GetUrl(OrderBookEndpoint), "GET", parameters).Result;
            return new CallResult<KunaOrderBook>(result.Data, result.Error);
        }

        public CallResult<List<KunaTrade>> GetTrades(string market)
        {
            var parameters = new Dictionary<string, object>() { { "market", market } };
            var result = ExecuteRequest<List<KunaTrade>>(GetUrl(AllTradesEndpoint), "GET", parameters).Result;
            return new CallResult<List<KunaTrade>>(result.Data, result.Error);
        }
        public CallResult<KunaAccountInfo> GetAccountInfo()
        {      
            var result = ExecuteRequest<KunaAccountInfo>(GetUrl(AccountInfoEndpoint), "GET",null,true).Result;
            return new CallResult<KunaAccountInfo>(result.Data, result.Error);
        }

        public CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market)
        {
            throw new NotImplementedException();
        }

        public CallResult<KunaPlacedOrder> CancelOrder(long orderId)
        {
            throw new NotImplementedException();
        }

        public CallResult<List<KunaPlacedOrder>> GetActiveOrders(string market)
        {
            throw new NotImplementedException();
        }

        public CallResult<List<KunaTrade>> GetMyTrades(string market)
        {
            throw new NotImplementedException();
        }

        #region BaseMethodOverride
        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            var uriString = uri.ToString();
            if ((method == Constants.GetMethod || method == Constants.DeleteMethod || postParametersPosition == PostParameters.InUri) && parameters?.Any() == true)
            {
                uriString += "?" + parameters.CreateParamString(true);
            }
            var request = RequestFactory.Create(uriString);
            request.ContentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            request.Accept = Constants.JsonContentHeader;
            request.Method = method;
            //var headers = new Dictionary<string, string>();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(new Uri(uriString).PathAndQuery, method, parameters,  signed);

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
            return version == null ? new Uri($"{BaseAddress}/{endpoint}") : new Uri($"{BaseAddress}/v{version}/{endpoint}");
        }

   

        #endregion

    }
}
