// See https://aka.ms/new-console-template for more information
using Kuna.Net.Interfaces;
using Kuna.Net;
IKunaClient kunaClient = new KunaClient(new KunaClientOptions() );
var or = kunaClient.ClientV2.GetMyOrdersV2("btcusdt",Kuna.Net.Objects.V2.KunaOrderStateV2.Done);
var trades = kunaClient.ClientV2.GetMyTradesV2("btcusdt");

var book = new KunaSymbolOrderBook("btcusdt", new KunaSocketClient(), new KunaSymbolOrderBookOptions(kunaClient, TimeSpan.FromSeconds(1)));
book.OnBestOffersChanged += Book_OnBestOffersChanged;

void Book_OnBestOffersChanged((CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry BestBid, CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry BestAsk) obj)
{
    Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}] {obj.BestBid.Price}:{obj.BestAsk.Price}");
}

await book.StartAsync();
Console.ReadLine();
Console.WriteLine("Done");