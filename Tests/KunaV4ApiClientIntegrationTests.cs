using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Kuna.Net;
using Kuna.Net.Objects.V4;
using Kuna.Net.Objects.V4.Requests;
using Microsoft.Extensions.Logging;
using Xunit;
using CryptoExchange.Net.Authentication;

namespace Kuna.Net.Tests
{
    [TestCaseOrderer(
    ordererTypeName: "Kuna.Net.Tests.PriorityOrderer",
    ordererAssemblyName: "Tests")]
    public class KunaV4ApiClientIntegrationTests
    {
        private readonly KunaV4ApiClient _apiClient;
        private static readonly Guid ordId = Guid.NewGuid();
        public KunaV4ApiClientIntegrationTests()
        {
            var config = new ConfigurationBuilder().AddJsonFile("keys.json", optional: true).Build();
            var key = config["key"];
            var secret = config["secret"];
            var singleKey = config["sinle-api-key"];

            KunaApiCredentials? cred = null;
            if (!string.IsNullOrEmpty(singleKey))
            {
                cred = new KunaApiCredentials(singleKey);
            }
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(secret))
            {
                cred = new KunaApiCredentials(key, secret);
            }
            _apiClient = (KunaV4ApiClient)new KunaClient(
                new KunaClientOptions()
                    {
                        LogLevel = LogLevel.Trace,
                        ApiCredentials = cred
                    }).ClientV4;
        }

        // Add integration tests for each async method in your class

        [Fact]
        public async Task GetServerTimeAsyncTest()
        {
            // Arrange
            var today = DateTime.Now.Date;
            // Act
            var result = await _apiClient.GetServerTimeAsync();

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Equal(today, result.Data.Date);
        }


        [Fact]
        public async Task GetTickersAsyncTest()
        {
            // Arrange

            // Act
            var result = await _apiClient.GetTickersAsync();

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.IsType<KunaTickerV4>(result.Data.First());
        }
        [Fact]
        public async Task GetTickersAsyncForBTCUAH_BTCSUDT_Test()
        {
            // Arrange
            var symbol1 = "BTC_UAH";
            var symbol2 = "BTC_USDT";
            // Act
            var result = await _apiClient.GetTickersAsync(default, symbol1, symbol2);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, t => t.Pair == symbol1);
            Assert.Contains(result.Data, t => t.Pair == symbol2);
        }
        [Fact]
        public async Task GetCurrenciesAsyncTest()
        {
            // Arrange

            // Act
            var result = await _apiClient.GetCurrenciesAsync();

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, c => c.Type == CurrencyType.Crypto);
            Assert.Contains(result.Data, c => c.Type == CurrencyType.Fiat);

        }
        [Fact]
        public async Task GetCurrenciesAsyncOnlyCryptoTest()
        {
            // Arrange

            // Act
            var result = await _apiClient.GetCurrenciesAsync(CurrencyType.Crypto);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, c => c.Type == CurrencyType.Crypto);
            Assert.DoesNotContain(result.Data, c => c.Type == CurrencyType.Fiat);

        }
        [Fact]
        public async Task GetRecentPublicTradesAsyncTest()
        {
            // Arrange
            var pair = "BTC_UAH";
            var today = DateTime.Now.Date;
            // Act
            var result = await _apiClient.GetRecentPublicTradesAsync(pair);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, c => c.Pair == pair);
            Assert.Contains(result.Data, c => c.CreatedAt.Date == today);

        }

        [Fact]
        public async Task GetTradingPairsAsyncTest()
        {
            // Arrange
            var pair = "BTC_UAH";
            // Act
            var result = await _apiClient.GetTradingPairsAsync();

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, c => c.Pair == pair);
        }

        [Fact]
        public async Task GetOrderBookAsyncTest()
        {
            // Arrange
            var pair = "BTC_UAH";
            // Act
            var result = await _apiClient.GetOrderBookAsync(pair, OrderBookLevel.Five);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.True(result.Data.Asks.First().Price > result.Data.Bids.First().Price);
        }

        [Fact]
        public async Task SendAsProTest()
        {
            // Arrange
            var pair = "BTC_UAH";

            // Act
            _apiClient.SetProAccount(true);
            var result = await _apiClient.GetOrderBookAsync(pair, OrderBookLevel.Twenty);
            _apiClient.SetProAccount(false);
            var proHeaderEntry = result.RequestHeaders?.FirstOrDefault(kv => kv.Key == "account");
            // Assert
            Assert.True(result.Success);

            // Add more assertions based on the expected behavior of this method
            Assert.Contains(proHeaderEntry.Value.Value, p => p.Equals("pro"));
        }

        [Fact, TestPriority(1)]
        public async Task PlaceOrderAsyncTest()
        {
            // Arrange
            var symbol = "BTC_UAH";
            var price = 10m;
            var amount = 0.1m;
            var side = KunaOrderSideV4.Bid;
            var req = PlaceOrderRequestV4.LimitOrder(symbol, amount, side, price, ordId);

            // Act
            var result = await _apiClient.PlaceOrderAsync(req);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Equal(symbol, result.Data.Pair);
            Assert.Equal(amount, result.Data.Quantity);
            Assert.Equal(price, result.Data.Price);


        }

        [Fact, TestPriority(2)]
        public async Task GetOrderAsyncTest()
        {
            // Arrange
            var id = ordId;
            // Act
            var result = await _apiClient.GetOrderAsync(id);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Equal(id, result.Data.Id);
        }

        [Fact, TestPriority(2)]
        public async Task GetOrderTradesAsync()
        {
            // Act
            var result = await _apiClient.GetOrderTradesAsync(ordId);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
        }

        [Fact, TestPriority(3)]
        public async Task CancelOrderAsync()
        {
            
            // Act
            var result = await _apiClient.CancelOrderAsync(ordId);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.True(result.Data.Success);

        }

        [Fact]
        public async Task GetBalancesAsync()
        {
            
            // Act
            var result = await _apiClient.GetBalancesAsync();

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Contains(result.Data, c => c.Currency == "BTC");

        }
    }
}