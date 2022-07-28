// See https://aka.ms/new-console-template for more information
using Kuna.Net.Interfaces;
using Kuna.Net;
IKunaClient kunaClient =  new KunaClient();
var book = new KunaSymbolOrderBook("btcusdt", new KunaSymbolOrderBookOptions(new KunaSocketClient(), kunaClient, TimeSpan.FromSeconds(1)));
book.OnBestOffersChanged += Book_OnBestOffersChanged;

void Book_OnBestOffersChanged((CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry BestBid, CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry BestAsk) obj)
{
    Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss.fff}] {obj.BestBid.Price}:{obj.BestAsk.Price}");
}

await book.StartAsync();
Console.ReadLine();
Console.WriteLine("Done");