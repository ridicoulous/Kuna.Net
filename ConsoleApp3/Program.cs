using CryptoExchange.Net.Objects;
using Kuna.Net;
using Kuna.Net.Objects.V4;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
    private static KunaV4RestApiClient _apiClient;
    private static KunaSocketStream _socketClient;
    private readonly static Guid ordId = Guid.NewGuid();

    private static async Task Main(string[] args)
    {

        SetUpClients();





        // #region socket
        // public async Task TestSub()
        // {
        // Arrange 
        var symbol = "BTC_USDT";
        // var topic = $"{symbol}@depth";
        var topic = $"arrTicker";
        string pair;
        // Act
        var s = await _socketClient.SubscribeInternal<JToken>(topic, ordBook =>
        {
            // Console.WriteLine(ordBook);
            // Console.WriteLine($"ord book come for {ordBook.Pair}: asks {string.Join(",", JsonConvert.SerializeObject(ordBook.Asks))} bids {string.Join(",", JsonConvert.SerializeObject(ordBook.Bids))}");
            // pair = ordBook.Pair;
        });
        // await _socketClient.UnsubscribeAsync(s.Data);
        Thread.Sleep(Timeout.Infinite);
        // Assert
        // expect that all the responses were successfully completed
        // Assert.True(done >= expectedSuccessfullyExecutedAmount);
        // and that all the responses were sent in a minute (ratelimeter should allow 1200req/min)
        // Assert.True(watch.ElapsedMilliseconds < 1000 * 60);
        // }
        // #endregion
    }

    private static void SetUpClients()
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
        var loggerFactory = LoggerFactory.Create(builder =>
          {
              builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddProvider(new TraceLoggerProvider());
          });
        _apiClient = (KunaV4RestApiClient)new KunaClient(
            new KunaRestOptions()
            {
                OutputOriginalData = true,
                // IsProAccount = true,
                ApiCredentials = cred
            },
            loggerFactory
        ).ClientV4;

        _socketClient = new KunaSocketClient(new KunaSocketClientOptions(), loggerFactory).MainSocketStreams;


    }
}