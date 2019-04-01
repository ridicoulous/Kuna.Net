using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuna.Net.SampleApp
{
    class Program
    {
        static IKunaClient _kunaClient = new KunaClient(new KunaClientOptions() { });
        static void Main(string[] args)
        {
            //IKunaClient client = new KunaClient(new KunaClientOptions()
            //{
            //    ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("","")
            //});
            //IKunaSocketClient socket = new KunaSocketClient(new KunaSocketClientOptions());
            //socket.SubscribeToOrderBookSideUpdates("btcuah", OnTrade);
           var btcuah = GetTrades("btcuah").ToList();
            // var me = client.GetAccountInfo();
            Console.ReadLine();
        }

        private static void OnTrade(KunaOrderBookUpdateEvent obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj));

        }

        private static void OnTrade(KunaTradeEvent arg1, string arg2)
        {
            Console.WriteLine(arg2 + " " + JsonConvert.SerializeObject(arg1));
        }

        private static IEnumerable<List<KunaTrade>> GetTrades(string market)
        {
            DateTime from = DateTime.UtcNow.AddMonths(-3);
            while (from < DateTime.UtcNow)
            {
                var data = _kunaClient.GetTrades(market, from);
                if (data.Success)
                {
                    from = data.Data.Max(c => c.Timestamp);
                    Console.WriteLine(from);
                    yield return data.Data;
                }
                else
                {
                    Console.WriteLine("LastDate: ");
                    Console.WriteLine(data.Error.Message);
                    break;
                }
            }
        }
    }
}
