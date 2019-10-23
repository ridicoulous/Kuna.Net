using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        ApiCredentials creds;
        public KunaAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            creds = credentials;
            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
        }
        public override Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
        {
            encryptorv3 = new HMACSHA384(Encoding.ASCII.GetBytes(creds.Secret.GetString()));
            if (!signed)
                return new Dictionary<string, string>();

            var result = new Dictionary<string, string>();

            if (uri.Contains("v3"))
            {

                result.Add("kun-nonce", Credentials.Key.GetString());

                result.Add("kun-apikey", Credentials.Key.GetString());
                result.Add("kun-signature", Credentials.Key.GetString());


                string jsonContent = "";
                if (method != HttpMethod.Get&& method != HttpMethod.Delete)
                    jsonContent = JsonConvert.SerializeObject(parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
                result.Add("Api-Content-Hash", ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(jsonContent))).ToLower());
            }
            //uri = WebUtility.UrlDecode(uri); // Sign needs the query parameters to not be encoded
            //var sign = result["Api-Timestamp"] + uri + method + result["Api-Content-Hash"] + "";
            //result.Add("Api-Signature", ByteToString(encryptorHmac.ComputeHash(Encoding.UTF8.GetBytes(sign))));
            return result;
        }
        public override Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
        {
            if (!signed)
                return parameters;
            //   var uriObj = new Uri(uri);
            if (uri.Contains("v2"))
            {
                parameters.Add("access_key", Credentials.Key.GetString());
                parameters.Add("tonce", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
              

                parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);

                var paramString = parameters.CreateParamString(false,ArrayParametersSerialization.MultipleValues);
                var signData = method + "|";
                signData += uri + "|";
                signData += paramString;
                byte[] signBytes;
                lock (encryptLock)
                    signBytes = encryptor.ComputeHash(Encoding.UTF8.GetBytes(signData));
                parameters.Add("signature", ByteArrayToString(signBytes));

                //if (method != Constants.GetMethod)
                //    foreach (var kvp in parameters)
                //        parameters.Add(kvp.Key, kvp.Value);
            }
            else if (uri.Contains("v3"))
            {
            }
            return parameters;
        }

        //public override string Sign(string toSign)
        //{
        //    return base.Sign(toSign);
        //}

        //public override byte[] Sign(byte[] toSign)
        //{
        //    return base.Sign(toSign);
        //}
        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
