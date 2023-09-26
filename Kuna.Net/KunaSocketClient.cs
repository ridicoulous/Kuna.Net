using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V4;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaSocketClient : BaseSocketClient, ISocketClient, IKunaSocketClientStreamV4
    {
        internal ILogger Logger { get => _logger; }
        /// <summary>
        /// Creates websocket client. Use MainSocketStreams property to work with websocket.
        /// If you need authentication, use only so called single api key, provide it in options properties
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public KunaSocketClient(KunaSocketClientOptions options, ILoggerFactory logger = null) : base(logger, "KunaSocketClient")
        {

            Initialize(options.CommonStreamsOptions);
            MainSocketStream = AddApiClient(new KunaSocketStream(_logger, options.CommonStreamsOptions, options));
        }

        public KunaSocketStream MainSocketStream { get; private set; }


    }

}
