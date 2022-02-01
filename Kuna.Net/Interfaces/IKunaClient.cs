using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient:IRestClient
    {
        /// <summary>
        /// Obsolete API v2 client
        /// </summary>
        IKunaApiClientV2 ClientV2 { get; }
        IKunaApiClientV3 ClientV3 { get; }
        ISpotClient CommonSpotClient { get; }

    }
}
