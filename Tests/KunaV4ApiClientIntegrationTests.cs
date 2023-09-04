using System.Threading.Tasks;
using Kuna.Net;
using Kuna.Net.Objects.V4;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests
{
    public class KunaV4ApiClientIntegrationTests
    {
        private readonly KunaV4ApiClient _apiClient;

        public KunaV4ApiClientIntegrationTests()
        {
            // Initialize your KunaV4ApiClient with appropriate configuration and credentials
            // You may need to provide mock credentials for testing or use a test environment.
            _apiClient = (KunaV4ApiClient)new KunaClient(new KunaClientOptions(){LogLevel = LogLevel.Trace}).ClientV4;
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
        public async Task GetTickersAsyncForBTC_UAH_Test()
        {
            // Arrange
            var symbol = "BTC_UAH";
            // Act
            var result = await _apiClient.GetTickersAsync(symbols: symbol);

            // Assert
            Assert.True(result.Success);
            // Add more assertions based on the expected behavior of this method
            Assert.Equal(symbol, result.Data.First().Pair);
        }

        // Repeat the above pattern for each async method in your class

        // [Fact]
        // public async Task PlaceOrderAsyncTest()
        // {
        //     // Arrange

        //     // Act
        //     var result = await _apiClient.PlaceOrderAsync(/* provide necessary parameters */);

        //     // Assert
        //     Assert.True(result.Success);
        //     // Add more assertions based on the expected behavior of this method
        // }

        // Create integration tests for other methods following the same pattern
    }
}