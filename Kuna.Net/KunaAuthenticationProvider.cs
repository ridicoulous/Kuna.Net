using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Kuna.Net
{
    public class KunaAuthenticationProvider : AuthenticationProvider
    {
        private readonly HMACSHA256 encryptor;
        private readonly object encryptLock = new object();
        public KunaAuthenticationProvider(ApiCredentials credentials) : base(credentials)
        {
            encryptor = new HMACSHA256(Encoding.ASCII.GetBytes(credentials.Secret.GetString()));
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
    }
}
