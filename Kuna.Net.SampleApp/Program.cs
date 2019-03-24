using Kuna.Net.Interfaces;
using System;

namespace Kuna.Net.SampleApp
{   
    class Program
    {
        static void Main(string[] args)
        {
            IKunaClient client = new KunaClient(new KunaClientOptions()
            {
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("","")
            });
          
            var me = client.GetAccountInfo();
        }

    }
}
