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
        private readonly object encryptLock = new object();
        private readonly HMACSHA384 encryptor3;
        private readonly object locker;
        public KunaAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            if (credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret needed");
            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
            encryptor3 = new HMACSHA384(Encoding.UTF8.GetBytes(credentials.Secret.GetString()));
        }

        public override Dictionary<string, object> AddAuthenticationToParameters(string uri, string method, Dictionary<string, object> parameters, bool signed)
        {         
            if (!signed)
                return parameters;
         //   var uriObj = new Uri(uri);
            parameters.Add("access_key", Credentials.Key.GetString());
            parameters.Add("tonce", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);

            var paramString = parameters.CreateParamString(false);
            var signData = method + "|";        
            signData += uri+ "|";
            signData += paramString;
            byte[] signBytes;
            lock (encryptLock)
                signBytes = encryptor.ComputeHash(Encoding.UTF8.GetBytes(signData));
            parameters.Add("signature", ByteArrayToString(signBytes));

            //if (method != Constants.GetMethod)
            //    foreach (var kvp in parameters)
            //        parameters.Add(kvp.Key, kvp.Value);

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
        public  Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
        {
            if (Credentials.Key == null)
                throw new ArgumentException("ApiKey/Secret needed");

            var result = new Dictionary<string, string>();
            if (!signed)
                return result;

            if (uri.Contains("v1"))
            {
                var signature = JsonConvert.SerializeObject(parameters);

                var payload = Convert.ToBase64String(Encoding.ASCII.GetBytes(signature));
                var signedData = Sign(payload);

                result.Add("X-BFX-APIKEY", Credentials.Key.GetString());
                result.Add("X-BFX-PAYLOAD", payload);
                result.Add("X-BFX-SIGNATURE", signedData.ToLower());
            }
            else if (uri.Contains("v2"))
            {
                var json = JsonConvert.SerializeObject(parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
              
                var signature = $"/api{uri.Split(new[] { ".com" }, StringSplitOptions.None)[1]}42{json}";
                var signedData = Sign(signature);

                result.Add("bfx-apikey", Credentials.Key.GetString());
                result.Add("bfx-nonce", "42");
                result.Add("bfx-signature", signedData.ToLower());
            }

            return result;
        }
        public override string Sign(string toSign)
        {
            lock (locker)
                return ByteToString(encryptor.ComputeHash(Encoding.UTF8.GetBytes(toSign)));
        }

    }
}
