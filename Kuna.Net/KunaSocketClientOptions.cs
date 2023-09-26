using System;
using CryptoExchange.Net.Objects.Options;

namespace Kuna.Net
{
    public class KunaSocketClientOptions : SocketApiOptions
    {
        // public KunaSocketClientOptions()
        // {
        //     ApiCredentials
        // }
        // public static KunaSocketClientOptions Default { get; set; } = new()
        // {
        // };
        public SocketExchangeOptions CommonStreamsOptions { get; private set; } = new() 
            {
                // DelayAfterConnect = TimeSpan.FromSeconds(2),
                // OutputOriginalData = true,
                AutoReconnect = true,
                SocketNoDataTimeout = TimeSpan.FromSeconds(20)
            };
    }
}
