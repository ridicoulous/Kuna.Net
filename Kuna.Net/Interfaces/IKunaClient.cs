using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClient:IRestClient
    {
        IKunaApiClientV4 ClientV4 { get; }
        ISpotClient CommonSpotClient { get; }

    }
}
