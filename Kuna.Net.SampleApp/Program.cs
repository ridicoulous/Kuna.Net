using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;

namespace Kuna.Net.SampleApp
{   
    class Program
    {
        static void Main(string[] args)
        {
            //IKunaClient client = new KunaClient(new KunaClientOptions()
            //{
            //    ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("","")
            //});
            IKunaSocketClient socket = new KunaSocketClient(new KunaSocketClientOptions());
            socket.SubscribeToOrderBookSideUpdates("btcuah",OnTrade);
            // var me = client.GetAccountInfo();
            Console.ReadLine();
        }

        private static void OnTrade(KunaOrderBookUpdateEvent obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj));

        }

        private static void OnTrade(KunaTradeEvent arg1, string arg2)
        {
            Console.WriteLine(arg2+" "+JsonConvert.SerializeObject(arg1));
        }
    }
}
