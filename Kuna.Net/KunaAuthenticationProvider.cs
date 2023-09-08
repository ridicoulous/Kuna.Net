using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Kuna.Net
{
    public class KunaAuthenticationProvider : AuthenticationProvider
    {
        private readonly object encryptLock = new();

        private static readonly object nonceLock = new();
        private static long lastNonce;
        internal static string Nonce
        {
            get
            {
                lock (nonceLock)
                {
                    var nonce = (long)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
                    if (nonce == lastNonce)
                        nonce += 1;

                    lastNonce = nonce;
                    return lastNonce.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        
        private readonly bool useSingleApiKey;
        public KunaAuthenticationProvider(ApiCredentials credentials, bool useSingleApiKey = false) : base(credentials)
        {
            this.useSingleApiKey = useSingleApiKey;
            // encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
            // encryptorv3 = new HMACSHA384(Encoding.ASCII.GetBytes(creds.Secret.GetString()));
        }

        public  Dictionary<string, string> AddAuthenticationToHeadersV4(Uri uri, SortedDictionary<string, object> parameters, bool signed)
        {
            if (!signed)
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>();            
            var json = JsonConvert.SerializeObject(parameters);

            if(useSingleApiKey)
            {
                result.Add("api-key", _credentials.Secret.GetString());
            }
            else
            {
                result.Add("public-key", _credentials.Key.GetString());
                var n = Nonce;
                result.Add("signature", SignV4($"{uri.PathAndQuery}{n}{json}").ToLower());
                result.Add("nonce", n);
            }
            return result;
        }


        public string SignV4(string toSign)
        {
            lock (encryptLock)
                return SignHMACSHA384(toSign);
        }    

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters,
            bool auth, ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
        {
            // var isProV4 = providedParameters.Remove(Objects.V4.KunaV4ApiClient.ProParameter.Key);

            //I guess it can be removed successfully after updating base lib
            // providedParameters = providedParameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);

            bodyParameters = new();
            uriParameters = new();
            headers = new();
            if (parameterPosition == HttpMethodParameterPosition.InBody && method == HttpMethod.Post)
            {
                bodyParameters = new SortedDictionary<string, object>(providedParameters);
            }
            if (parameterPosition == HttpMethodParameterPosition.InUri && method == HttpMethod.Get)
                uriParameters = new SortedDictionary<string, object>(providedParameters);

            if(auth)
            {
                if (uri.AbsolutePath.Contains("v4"))
                {
                    headers = AddAuthenticationToHeadersV4(uri, bodyParameters, auth);
                }
            }
            
        }
    }
}
