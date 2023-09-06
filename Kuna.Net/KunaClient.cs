using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V3;
using Kuna.Net.Objects.V4;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaClient : BaseRestClient, IKunaClient
    {
        private static readonly KunaApiClientOptions DefaultOptions = new(false,true);
        private static readonly KunaApiClientOptions DefaultOptionsV2 = new(false,false);
        private static readonly KunaClientOptions DefaultBaseOptions = new(false);

        public KunaClient(KunaClientOptions exchangeOptions) : base("Kuna", exchangeOptions)
        {
            ClientV2 = AddApiClient(new KunaV2ApiClient(log, this, exchangeOptions, DefaultOptionsV2));
            ClientV3 = AddApiClient(new KunaV3ApiClient(log, this, exchangeOptions, DefaultOptions));
            ClientV4 = AddApiClient(new KunaV4ApiClient(log, this, exchangeOptions, DefaultOptions));
        }

        public KunaClient() : this(DefaultBaseOptions)
        {
        }

        public string ExchangeName => "Kuna";

        public IKunaApiClientV2 ClientV2 { get; }

        public IKunaApiClientV3 ClientV3 { get; }
        public IKunaApiClientV4 ClientV4 { get; }


        public ISpotClient CommonSpotClient => ClientV4;

        internal async Task<WebCallResult<T>> SendRequestInternal<T>(RestApiClient apiClient,
                                                                     Uri uri,
                                                                     HttpMethod method,
                                                                     CancellationToken cancellationToken,
                                                                     Dictionary<string, object>? parameters = null,
                                                                     bool signed = false,
                                                                     HttpMethodParameterPosition? postPosition = null,
                                                                     ArrayParametersSerialization? arraySerialization = null,
                                                                     int weight = 1) where T : class
        {
            return await base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, requestWeight: weight);
        }

    }
}
