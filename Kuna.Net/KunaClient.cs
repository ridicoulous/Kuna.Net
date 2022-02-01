using CryptoExchange.Net;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Kuna.Net.Converters;
using Kuna.Net.Helpers;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects.V2;
using Kuna.Net.Objects.V3;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaClient : BaseRestClient, IKunaClient
    {   
        public KunaClient(string exchangeName, BaseRestClientOptions exchangeOptions) : base(exchangeName, exchangeOptions)
        {
        }

        public string ExchangeName => "Kuna";

        public IKunaApiClientV2 ClientV2 => throw new NotImplementedException();

        public IKunaApiClientV3 Client => throw new NotImplementedException();

        public ISpotClient CommonSpotClient => Client;


        internal Task<WebCallResult<T>> SendRequestInternal<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken cancellationToken,
   Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
   ArrayParametersSerialization? arraySerialization = null, int weight = 1) where T : class
        {
            return base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization, requestWeight: weight);
        }

    }
}
