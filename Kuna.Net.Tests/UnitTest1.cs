using Kuna.Net.Interfaces;
using System;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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
        IKunaClient client = new KunaClient();
        [Fact(DisplayName = "ServerTime")]
        public void ShouldGetServerTime()
        {
            var serverTime = client.GetServerTime();
            Assert.True(Math.Abs(serverTime.Data.Subtract( DateTime.UtcNow).Seconds)<2);
        }
        [Fact(DisplayName = "GetMarketInfo")]
        public void ShouldGetMarketInfo()
        {
            var marketInfo = client.GetMarketInfo("btcusdt");
            Assert.True(marketInfo);
        }
        [Fact(DisplayName = "OrderBook")]
        public void SholdGetOrderbook()
        {
            var orderbook = client.GetOrderBook("btcusdt");
            Assert.True(orderbook);
            
            orderbook.Data.Asks.ShouldNotBeNull();
            orderbook.Data.Bids.ShouldNotBeNull();
        }
        [Fact(DisplayName = "TradesHistory")]
        public void ShoulGetTradesHistory()
        {
            var trades = client.GetTrades("btcusdt");
            Assert.True(trades);
            trades.Data.ShouldNotBeNull();
            trades.Error.ShouldBeNull();
        }

        [Fact(DisplayName = "Account")]

        public void ShouldGetAccountInfo()
        {
            var _client = GetClientWithAuthentication();
            var accountData = _client.GetAccountInfo();
            Assert.True(accountData);
            accountData.Data.ShouldNotBeNull();
        }

        [Fact(DisplayName = "CandlesHistoryAsync")]
        public async Task ShouldGetCandlesHistoryAsync()
        {
            var date = new DateTime(2020, 01, 01);
            var date1 = date.AddDays(-1);
            var date2 = date.AddDays(0);
            var history = await client.GetCandlesHistoryAsync("btcusd", 60, date1, date2);

            Assert.True(history);
            history.Data.ShouldNotBeNull();
            history.Data.Count.ShouldBe(25);
            history.Error.ShouldBeNull();
        }

        private IKunaClient GetClientWithAuthentication()
        {
            var config = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
            var key = config["key"];
            var secret = config["secret"];
            var client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(key, secret)
            }) ;
            return client;            
        }
    }
}
