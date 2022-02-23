using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V3;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaClient : BaseRestClient, IKunaClient
    {
        private static KunaApiClientOptions DefaultOptions = new KunaApiClientOptions(false,true);
        private static KunaApiClientOptions DefaultOptionsV2 = new KunaApiClientOptions(false,false);
        private static KunaClientOptions DefaultBaseOptions = new KunaClientOptions(false);

        public KunaClient(KunaClientOptions exchangeOptions) : base("Kuna", exchangeOptions)
        {
            ClientV2 = AddApiClient(new KunaV2ApiClient(log, this, exchangeOptions, DefaultOptionsV2));
            ClientV3 = AddApiClient(new KunaApiClient(log, this, exchangeOptions, DefaultOptions));
        }

        public KunaClient() : base("Kuna", DefaultBaseOptions)
        {
            ClientV2 = AddApiClient(new KunaV2ApiClient(log, this, DefaultBaseOptions, DefaultOptionsV2));
            ClientV3 = AddApiClient(new KunaApiClient(log, this, DefaultBaseOptions, DefaultOptions));
        }
        public string ExchangeName => "Kuna";

        public IKunaApiClientV2 ClientV2 { get; }

        public IKunaApiClientV3 ClientV3 { get; }

        public ISpotClient CommonSpotClient => ClientV3;

        internal async Task<WebCallResult<T>> SendRequestInternal<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken cancellationToken,
   Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
   ArrayParametersSerialization? arraySerialization = null, int weight = 1) where T : class
        {
            return await base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, requestWeight: weight);
        }

    }
}
