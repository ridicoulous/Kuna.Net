using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Objects
{
    public class KunaApiCallError : Error
    {
        public KunaApiCallError(int code, string message) : base(code, message,null)
        {
        }
    }
}
