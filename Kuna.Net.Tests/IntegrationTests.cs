using System;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CryptoExchange.Net.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;

namespace Kuna.Net.Tests
{
    public class IntegrationTests
    {
        KunaClient client;
        public IntegrationTests()
        {
            client = GetClientWithAuthentication(false);

        }
        [Fact(DisplayName = "PlaceOrder")]
        public  async Task PlaceOrder()
        {
            var order = await client.ClientV3.PlaceOrderAsync("btcusdt", Objects.V3.KunaOrderSide.Sell, Objects.V3.KunaOrderType.Limit, 0.00001m, 188888m);

            Assert.True(order);

            var cancel = await client.CommonSpotClient.CancelOrderAsync(order.Data.Id.ToString());
            Assert.True(cancel);

        }
        [Fact(DisplayName = "Bulk")]

        public async Task BulkTest()
        {
            var book = await client.CommonSpotClient.GetOrderBookAsync("btcusdt");
            Assert.True(book);
            var activeOrders = await client.ClientV3.GetActiveOrdersAsync();
            Assert.True(activeOrders);
            //var closed = await client.ClientV3.GetClosedOrdersAsync();
            //Assert.True(closed);
            //var trades = await client.ClientV3.GetOrdersWithTradesAsync("btcusdt");
            //Assert.True(trades);
            var recentTrades = await client.ClientV3.GetRecentPublicTradesAsync("btcusdt");
            Assert.True(recentTrades);
            

        }

        [Fact(DisplayName = "GetMarketInfo")]
        public void ShouldGetMarketInfo()
        {
            var marketInfo = client.ClientV2.GetMarketInfoV2("btcusdt");
            Assert.True(marketInfo);
        }
        [Fact(DisplayName = "OrderBook")]
        public void SholdGetOrderbook()
        {
            var orderbook = client.ClientV2.GetOrderBookV2("btcusdt");
            Assert.True(orderbook);

            orderbook.Data.Asks.ShouldNotBeNull();
            orderbook.Data.Bids.ShouldNotBeNull();
        }
        [Fact(DisplayName = "TradesHistory")]
        public void ShoulGetTradesHistory()
        {
            var trades = client.ClientV2.GetTradesV2("btcusdt");
            Assert.True(trades);
            trades.Data.ShouldNotBeNull();
            trades.Error.ShouldBeNull();
        }

        [Fact(DisplayName = "Account")]

        public void ShouldGetAccountInfo()
        {
            var accountData = client.ClientV3.GetBalances();
            Assert.True(accountData);
            accountData.Data.ShouldNotBeNull();
        }

        [Fact(DisplayName = "CandlesHistoryAsync")]
        public async Task ShouldGetCandlesHistoryAsync()
        {
            var date = new DateTime(2020, 01, 01);
            var date1 = date.AddDays(-1);
            var date2 = date.AddDays(0);
            var history = await client.ClientV3.GetCandlesHistoryV2Async("btcusd", 60, date1, date2);

            Assert.True(history);
            history.Data.ShouldNotBeNull();
            //history.Data.Count.ShouldBe(25);
            //history.Error.ShouldBeNull();
        }

        private KunaClient GetClientWithAuthentication(bool pro)
        {
            var config = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
            var key = config["key"];
            var secret = config["secret"];
            

            ApiCredentials c = string.IsNullOrEmpty(key) ? null : new CryptoExchange.Net.Authentication.ApiCredentials(key, secret);
            var client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = c,

                LogLevel = LogLevel.Debug,
                
                LogWriters = new List<ILogger>() { new DebugLogger() , new ConsoleLogger() },
                IsProAccount = pro,
                //RateLimiters = new List<CryptoExchange.Net.Interfaces.IRateLimiter>() { new RateLimiterTotal(1100, TimeSpan.FromMinutes(1)) },
                //RateLimitingBehaviour = CryptoExchange.Net.Objects.RateLimitingBehaviour.Fail,
                RequestTimeout = TimeSpan.FromSeconds(4)


            });
            
            return client;
            
        }
    }
}
