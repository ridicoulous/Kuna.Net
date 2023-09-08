using System.Net.Http;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces.CommonClients;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V4;
using Microsoft.Extensions.Logging;

namespace Kuna.Net
{
    public class KunaClient : BaseRestClient, IKunaClient
    {
        private static readonly KunaApiClientOptions DefaultOptions = new();
        private static readonly KunaRestOptions DefaultBaseOptions = new(false);

        public KunaClient(KunaRestOptions exchangeOptions, ILoggerFactory logger = null, HttpClient httpClient = null) : base(logger, "Kuna")
        {
            var options = exchangeOptions ?? DefaultBaseOptions;
            Initialize(options);
            ClientV4 = AddApiClient(new KunaV4RestApiClient(_logger, httpClient, "https://api.kuna.io/", options, DefaultOptions));
        }

        public KunaClient() : this(DefaultBaseOptions)
        {
        }

        public string ExchangeName => "Kuna";

        public IKunaApiClientV4 ClientV4 { get; }


        public ISpotClient CommonSpotClient => ClientV4;


    }
}
