using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Objects;

namespace Kuna.Net
{
    public abstract class KunaBaseApiClient : RestApiClient
    {
        protected KunaClient _kunaClient;
        protected string versionSuffix = string.Empty;

        public virtual string ExchangeName => "Kuna";

        protected KunaBaseApiClient(BaseRestClientOptions options, RestApiClientOptions apiOptions) : base(options, apiOptions)
        {
        }

        /// <summary>
        /// Whether or not the request can be signed
        /// </summary>
        protected bool CanBeSigned => AuthenticationProvider is not null;
        protected Uri GetUrl(string endpoint)
        {
            return new Uri($"{BaseAddress}{(string.IsNullOrEmpty(versionSuffix)? string.Empty : $"{versionSuffix}/" )}{endpoint}");
        }
        protected async Task<WebCallResult<T>> SendRequestAsync<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object> request, bool signed, HttpMethodParameterPosition? position = null) where T : class
        {
            return await _kunaClient.SendRequestInternal<T>(this, uri, method, ct, request, signed, position);
        }

        /// <summary>
        /// Fill parameters in a path. Parameters are specified by '{}' and should be specified in occuring sequence
        /// </summary>
        /// <param name="path">The total path string</param>
        /// <param name="values">The values to fill</param>
        /// <returns></returns>
        protected static string FillPathParameter(string path, params string[] values)
        {
            foreach (var value in values)
            {
                var indexB = path.IndexOf("{", StringComparison.Ordinal);
                var indexE = path.IndexOf("}", StringComparison.Ordinal);
                if (indexB >= 0 && indexE > indexB)
                {
                    path = path.Remove(indexB, indexE - indexB + 1);
                    path = path.Insert(indexB, value);
                }
            }
            return path;
        }
   }
}