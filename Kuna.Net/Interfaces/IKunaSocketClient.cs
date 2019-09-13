using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Objects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kuna.Net.Interfaces
{
    public interface IKunaSocketClient:IDisposable
    {
        void SubscribeToOrderBookSideUpdates(string market, Action<KunaOrderBookUpdateEvent, string> onUpdate);
        void SubscribeToTrades(string market, Action<KunaTradeEvent,string> onUpdate);
        CallResult<UpdateSubscription> CreateFakeSubsctiption();

        Task<CallResult<UpdateSubscription>> CreateFakeSubsctiptionAsync();
    }
}
