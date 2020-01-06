using Kuna.Net.Interfaces;
using System;
using Xunit;
using Shouldly;
namespace Kuna.Net.Tests
{
    public class UnitTest1
    {
        //CallResult<List<KunaTrade>> GetTrades(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");

        //CallResult<KunaAccountInfo> GetAccountInfo();

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
            //var serverTime = client.GetServerTime();            
            //Assert.True(serverTime.Data>DateTime.UtcNow.AddSeconds(-1));

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
            var orderbook = client.GetOrderBook("btcusdt");
            Assert.True(orderbook);
            orderbook.Data.Asks.ShouldNotBeNull();
            orderbook.Data.Bids.ShouldNotBeNull();
        }

        [Theory(DisplayName ="Account")]
        [InlineData("","")]
        public void ShouldGetAccountInfo(string key, string secret)
        {

        }
    }
}
