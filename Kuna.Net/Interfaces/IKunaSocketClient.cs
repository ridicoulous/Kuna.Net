using Kuna.Net.Objects;
using System;
using System.Collections.Generic;

namespace Kuna.Net.Interfaces
{
    public interface IKunaSocketClient:IDisposable
    {
        void SubscribeToOrderBookSideUpdates(string market, Action<KunaOrderBookUpdateEvent, string> onUpdate);
        void SubscribeToTrades(string market, Action<KunaTradeEvent,string> onUpdate);
        
    }
}
