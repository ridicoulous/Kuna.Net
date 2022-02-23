using Kuna.Net.Objects.V2;
using System;

namespace Kuna.Net.Interfaces
{
    public interface IKunaSocketClientV2:IDisposable
    {
        void SubscribeToOrderBookSideUpdatesV2(string market, Action<KunaOrderBookUpdateEventV2, string> onUpdate);
        void SubscribeToTradesV2(string market, Action<KunaTradeEventV2,string> onUpdate);     
    }
}
