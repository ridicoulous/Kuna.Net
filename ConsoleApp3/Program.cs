using CryptoExchange.Net.Objects;
using Kuna.Net;
using Kuna.Net.Objects.V4;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
//     private static KunaV4RestApiClient? _restSubClient;
//     private static KunaSocketStream? _socketSubClient;
    private static KunaRestClient? _restClient;
    private static KunaSocketClient? _socketClient;
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
        var s = await _socketClient.MainSocketStream.SubscribeToBalances(ordBook =>
        {
            foreach(var e in ordBook)
            {
                Console.WriteLine(e.ToString());

            }
            // Console.WriteLine($"ord book come for {ordBook.Pair}: asks {string.Join(",", JsonConvert.SerializeObject(ordBook.Asks))} bids {string.Join(",", JsonConvert.SerializeObject(ordBook.Bids))}");
            // pair = ordBook.Pair;
        });
        // var orderbook = new KunaSymbolOrderBook("BTC_USDT", _socketClient, _restClient);
        // await orderbook.StartAsync();
        // orderbook.OnBestOffersChanged += book =>
        // {
        //     Console.WriteLine($"best ask: {JsonConvert.SerializeObject(book.BestAsk)}, best bid: {JsonConvert.SerializeObject(book.BestBid)}");
        // };


        // await _socketClient.UnsubscribeAsync(s.Data);
        Thread.Sleep(Timeout.Infinite);
 
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
        _restClient = new KunaRestClient(
            new KunaRestOptions()
            {
                OutputOriginalData = true,
                // IsProAccount = true,
                ApiCredentials = cred
            },
            loggerFactory
        );

        _socketClient = new KunaSocketClient(new KunaSocketClientOptions() {ApiCredentials = cred }, loggerFactory);


    }
}