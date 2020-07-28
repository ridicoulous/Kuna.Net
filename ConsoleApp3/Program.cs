using Kuna.Net;
using System;

namespace ConsoleApp3
{
    class Program
    {
        static KunaSocketClient kunaSocketClient = new KunaSocketClient();
        static KunaSymbolOrderBook ob = new KunaSymbolOrderBook("usdtuah", kunaSocketClient, new KunaSymbolOrderBookOptions("asd"));
        static void Main(string[] args)
        {
            
            ob.OnOrderBookUpdate += Ob_OnOrderBookUpdate;
            ob.Start();
            Console.ReadLine();
        }

        private static void Ob_OnOrderBookUpdate(System.Collections.Generic.IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> arg1, System.Collections.Generic.IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> arg2)
        {
            Console.WriteLine("ip");
        }
    }
}
