using CryptoExchange.Net.Objects;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V3;
using System;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaSymbolOrderBookOptions : OrderBookOptions
    {
        public readonly TimeSpan UpdateTimeout;
        public readonly IKunaClient? KunaClient;
        public readonly KunaSocketClient KunaSocketClient;
        public TimeSpan CheckBookTimeout { get; set; }= TimeSpan.FromSeconds(7);
        public KunaSymbolOrderBookOptions(KunaSocketClient kunaSocketClient, IKunaClient? kunaApiClient = null, TimeSpan? timeout = null)
        {            
            KunaSocketClient = kunaSocketClient;
            KunaClient = kunaApiClient;
            UpdateTimeout = timeout ?? TimeSpan.FromSeconds(1.75);
        }
    }
}
