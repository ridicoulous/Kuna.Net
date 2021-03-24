using Kuna.Net.Interfaces;
using System;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CryptoExchange.Net.Logging;

namespace Kuna.Net.Tests
{
    public class UnitTest1
    {        

        //CallResult<KunaPlacedOrder> PlaceOrder(OrderType type, OrderSide side, decimal volume, decimal price, string market);

        //CallResult<KunaPlacedOrder> CancelOrder(long orderId);

        //CallResult<List<KunaPlacedOrder>> GetMyOrders(string market, OrderState orderState = OrderState.Wait, int page = 1, string sort = "desc");

        //CallResult<KunaPlacedOrder> GetOrderInfo(long orderId);

        //CallResult<List<KunaTrade>> GetMyTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");

        //CallResult<List<KunaTraidingPair>> GetExchangeCurrenciesInfo();
        //Task<CallResult<List<KunaOhclv>>> GetCandlesHistoryAsync(string symbol, int resolution, DateTime from, DateTime to, CancellationToken token = default);
        IKunaClientV2 client = new KunaClient();
        // [Fact(DisplayName = "ServerTime")]
        // public void ShouldGetServerTime()
        // {
        //     var serverTime = client.GetServerTimeV2();
        //     Assert.True(Math.Abs(serverTime.Data.Subtract( DateTime.UtcNow).Seconds)<2);
        //     var c = client.GetCurrenciesV2();
        //     Assert.True(c);
        // }
        [Fact(DisplayName = "PlaceORder")]
        public void PlaceOrder()
        {
            var c = GetClientWithAuthentication();
            var o = c.PlaceOrder("btcusdt", Objects.V3.KunaOrderSide.Buy, Objects.V3.KunaOrderType.Limit, 1, 1);
            if(o)
            {
                var cancel = c.CancelOrder(o.Data.Id);
                var order = c.GetOrders(Objects.V3.KunaOrderStatus.Done);
                Assert.True(order);
            }
          
        }
        [Fact(DisplayName = "GetMarketInfo")]
        public void ShouldGetMarketInfo()
        {
            var marketInfo = client.GetMarketInfoV2("btcusdt");
            Assert.True(marketInfo);
        }
        [Fact(DisplayName = "OrderBook")]
        public void SholdGetOrderbook()
        {
            var orderbook = client.GetOrderBookV2("btcusdt");
            Assert.True(orderbook);
            
            orderbook.Data.Asks.ShouldNotBeNull();
            orderbook.Data.Bids.ShouldNotBeNull();
        }
        [Fact(DisplayName = "TradesHistory")]
        public void ShoulGetTradesHistory()
        {
            var trades = client.GetTradesV2("btcusdt");
            Assert.True(trades);
            trades.Data.ShouldNotBeNull();
            trades.Error.ShouldBeNull();
        }

        [Fact(DisplayName = "Account")]

        public void ShouldGetAccountInfo()
        {
            var _client = GetClientWithAuthentication();
            var accountData = _client.GetAccountInfoV2();
            Assert.True(accountData);
            accountData.Data.ShouldNotBeNull();
        }

        [Fact(DisplayName = "CandlesHistoryAsync")]
        public async Task ShouldGetCandlesHistoryAsync()
        {
            var date = new DateTime(2020, 01, 01);
            var date1 = date.AddDays(-1);
            var date2 = date.AddDays(0);
            var history = await client.GetCandlesHistoryV2Async("btcusd", 60, date1, date2);

            Assert.True(history);
            history.Data.ShouldNotBeNull();
            history.Data.Count.ShouldBe(25);
            history.Error.ShouldBeNull();
        }

        private KunaClient GetClientWithAuthentication()
        {
            var config = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
            var key = config["key"];
            var secret = config["secret"];
            var client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(key, secret),
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new System.Collections.Generic.List<System.IO.TextWriter>() { new DebugTextWriter(), new ThreadSafeFileWriter("debug-client.log") }

            }) ;
            return client;            
        }
    }
}
