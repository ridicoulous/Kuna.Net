using Kuna.Net.Interfaces;
using System;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CryptoExchange.Net.Logging;
using System.Linq;

namespace Kuna.Net.Tests
{
    public class IntegrationTests
    {
        KunaClient client;
        public IntegrationTests()
        {
            client = GetClientWithAuthentication(false);

        }
        [Fact(DisplayName = "PlaceORder")]
        public void PlaceOrder()
        {
            var ordersss = client.GetOrders(Objects.V3.KunaOrderStatus.Filled, "xrpusdt", limit: 100);
            var t = ordersss.Data.Where(o => o.Status != Objects.V3.KunaOrderStatus.Canceled).ToList();
            var o = client.PlaceOrder("btcusdt", Objects.V3.KunaOrderSide.Buy, Objects.V3.KunaOrderType.Limit, 1, 1);
            if (o)
            {               
                var orders = client.GetOrders(Objects.V3.KunaOrderStatus.Filled,"xrpusdt",limit:1000);
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
            var client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(key, secret),
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new System.Collections.Generic.List<System.IO.TextWriter>() { new DebugTextWriter(), new ThreadSafeFileWriter("debug-client.log") },
                IsProAccount = pro

            });
            return client;
        }
    }
}
