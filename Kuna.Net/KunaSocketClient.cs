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
        public KunaSocketClient(KunaSocketClientOptions options, ILoggerFactory logger = null) : base(logger, "KunaSocketClient")
        {

            Initialize(options.CommonStreamsOptions);
            MainSocketStreams = AddApiClient(new KunaSocketStream(_logger, options.CommonStreamsOptions, options));
        }

        public KunaSocketStream MainSocketStreams { get; private set; }


    }

}
