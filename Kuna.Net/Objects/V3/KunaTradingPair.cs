using System;
using CryptoExchange.Net.ExchangeInterfaces;
using Kuna.Net.Objects.V2;

namespace Kuna.Net.Objects.V3
{
    public class KunaTradingPair : KunaTraidingPairV2, ICommonSymbol
    {
        public string CommonName => Pair;

        /// <summary>
        /// there may be wrong value here if Quote currency 
        /// has less quantity in your order than allows Quote precision
        /// </summary>
        public decimal CommonMinimumTradeSize => (decimal)Math.Pow(0.1, BasePrecision);
    }
}