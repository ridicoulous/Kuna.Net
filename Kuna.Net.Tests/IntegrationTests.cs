using Kuna.Net.Interfaces;
using System;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CryptoExchange.Net.Logging;
using System.Linq;
using System.Collections.Generic;
using CryptoExchange.Net.RateLimiter;
using System.Threading;

namespace Kuna.Net.Tests
{
    public class IntegrationTests
    {
        KunaClient client;
        public IntegrationTests()
        {
            client = GetClientWithAuthentication(true);

        }
        [Fact(DisplayName = "PlaceORder")]
        public void PlaceOrder()
        {
            
            var tasks = new List<Task>();
            for (int i = 0; i < 602; i++)
            {
                Task.Run(async()=> await client.GetOrdersAsync(Objects.V3.KunaOrderStatus.Active));
                Thread.Sleep(5);
                Task.Run(async () => await client.GetOrderBookAsync("dashusdt"));
                Thread.Sleep(90);
            }
            //   Task.WaitAll(tasks.ToArray());
         
            var odfg = client.GetOrders(Objects.V3.KunaOrderStatus.Active);
            var book = client.GetOrderBook("btcusdt");
            var ordersss = client.GetOrders(Objects.V3.KunaOrderStatus.Filled, "xrpusdt", limit: 100);
            var t = ordersss.Data.Where(o => o.Status == Objects.V3.KunaOrderStatus.Filled).ToList();
            var o = client.PlaceOrder("btcusdt", Objects.V3.KunaOrderSide.Buy, Objects.V3.KunaOrderType.Limit, 1, 1);
            if (o)
            {
                var orders = client.GetOrders(Objects.V3.KunaOrderStatus.Filled, "xrpusdt", limit: 1000);
                var placed = client.GetOrder(o.Data.Id);
                var cancel = client.CancelOrder(o.Data.Id);
                Assert.True(orders);
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
            var accountData = client.GetBalances();
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

        private KunaClient GetClientWithAuthentication(bool pro)
        {
            var config = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
            var key = config["key"];
            var secret = config["secret"];
            CryptoExchange.Net.Authentication.ApiCredentials c = string.IsNullOrEmpty(key) ? null : new CryptoExchange.Net.Authentication.ApiCredentials(key, secret);
            var client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = c,
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new System.Collections.Generic.List<System.IO.TextWriter>() { new DebugTextWriter(), new ThreadSafeFileWriter("debug-client.log") },
                IsProAccount = pro,
                RateLimiters = new List<CryptoExchange.Net.Interfaces.IRateLimiter>() { new RateLimiterTotal(1100, TimeSpan.FromMinutes(1)) },
                RateLimitingBehaviour = CryptoExchange.Net.Objects.RateLimitingBehaviour.Fail,
                RequestTimeout = TimeSpan.FromSeconds(4)


            });
            return client;
        }
    }
}
