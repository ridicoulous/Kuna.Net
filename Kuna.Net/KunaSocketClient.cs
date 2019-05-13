using CryptoExchange.Net;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using PusherClient;
using System;
using System.Collections.Generic;

namespace Kuna.Net
{
    /*wss://pusher.kuna.io/app/4b6a8b2c758be4e58868?protocol=7&client=js&version=3.0.0&flash=false*/
    public class KunaSocketClient : SocketClient, IKunaSocketClient
    {
        private Pusher _pusherClient;
        public KunaSocketClient(KunaSocketClientOptions options):base(options,null)
        {
            _pusherClient = new Pusher("4b6a8b2c758be4e58868", new PusherOptions() { Encrypted = true, Endpoint = "pusher.kuna.io", ProtocolNumber = 7, Version = "3.0.0" });
            _pusherClient.Connect();

        }
        #region Channels
        /// <summary>
        /// need to send market, eg btcuah
        /// </summary>
        private const string MarketTradesChannel = "market-{}-global";

        #endregion
        public void SubscribeToOrderBookSideUpdates(string market, Action<KunaOrderBookUpdateEvent,string> onUpdate)
        {
            var _myChannel = _pusherClient.Subscribe(FillPathParameter(MarketTradesChannel, market));
            _myChannel.Bind("update", (dynamic data) =>
            {
                string t = Convert.ToString(data);          
                KunaOrderBookUpdateEvent deserialized = Deserialize<KunaOrderBookUpdateEvent>(t).Data;
                onUpdate(deserialized,market);
            });

        }

        public void SubscribeToTrades(string market, Action<KunaTradeEvent,string> onUpdate)
        {
            var _myChannel = _pusherClient.Subscribe(FillPathParameter(MarketTradesChannel, market));
            _myChannel.Bind("trades", (dynamic data) =>
            {
                string t = Convert.ToString(data);
                KunaTradeEvent deserialized = Deserialize<KunaTradeEvent>(t).Data;
                onUpdate(deserialized, market);
            });

        }

        protected override bool SocketReconnect(SocketSubscription subscription, TimeSpan disconnectedTime)
        {
            return false;
        }
    }
}
