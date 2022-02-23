using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Kuna.Net
{
    public class KunaAuthenticationProvider : AuthenticationProvider
    {
        private readonly HMACSHA256 encryptor;
        private HMACSHA384 encryptorv3;

        private readonly object encryptLock = new object();

        private static readonly object nonceLock = new object();
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
        ApiCredentials creds;
        public KunaAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            creds = credentials;
            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
            encryptorv3 = new HMACSHA384(Encoding.ASCII.GetBytes(creds.Secret.GetString()));
        }

        public  Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition postParameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>();

            if (uri.Contains("v3"))
            {                
                var json = JsonConvert.SerializeObject(parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
                if (string.IsNullOrEmpty(json)&&method==HttpMethod.Post)
                {
                    json = "{}";
                }
                var n = Nonce;
                var signature = $"{uri.Split(new[] { ".io" }, StringSplitOptions.None)[1]}{n}{json}";
                var signedData = SignV3(signature);

                result.Add("kun-apikey", Credentials.Key.GetString());
                result.Add("kun-nonce", n);
                result.Add("kun-signature", signedData.ToLower());
            }

            return result;
        }

        public  Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition postParameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return parameters;       
            if (uri.Contains("v2"))
            {
                parameters.Add("access_key", Credentials.Key.GetString());
                var n = Nonce;
                parameters.Add("tonce", n);

                parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);

                var paramString = parameters.CreateParamString(false, ArrayParametersSerialization.MultipleValues);
                var signData = method + "|";
                signData += uri + "|";
                signData += paramString;
                byte[] signBytes;
                lock (encryptLock)
                    signBytes = encryptor.ComputeHash(Encoding.UTF8.GetBytes(signData));
                parameters.Add("signature", ByteArrayToString(signBytes));
            }

            return parameters;
        }

        public string SignV3(string toSign)
        {
            lock (encryptLock)
                return ByteArrayToString(encryptorv3.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
        }    
        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters,
            bool auth, ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
        {
            var uriParam = new SortedDictionary<string, object>();
            bodyParameters = new();
            uriParameters = new();
            headers = new();
            if (parameterPosition == HttpMethodParameterPosition.InBody && method == HttpMethod.Post)
            {
                bodyParameters = new SortedDictionary<string, object>(providedParameters);
            }
            if (parameterPosition == HttpMethodParameterPosition.InUri && method == HttpMethod.Get)
                uriParam = new SortedDictionary<string, object>(providedParameters);

            if(auth)
            {
                if (uri.PathAndQuery.Contains("v3"))
                {
                    headers = AddAuthenticationToHeaders(uri.ToString(), method, providedParameters, auth, parameterPosition, arraySerialization);
                }
                else
                {
                    var uriAuthParam  = AddAuthenticationToParameters(uri.ToString(), method, providedParameters, auth, parameterPosition, arraySerialization);
                    foreach(var p in uriAuthParam)
                    {
                        uriParam.Add(p.Key, p.Value);
                    }

                }
            }
            uriParameters = uriParam;
            
        }
    }
}
