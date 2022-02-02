using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Interfaces;
using Kuna.Net.Objects;
using Kuna.Net.Objects.V2;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kuna.Net
{
    /*wss://pusher.kuna.io/app/4b6a8b2c758be4e58868?protocol=7&client=js&version=3.0.0&flash=false*/
    public class KunaSocketClient : BaseSocketClient, ISocketClient, IKunaSocketClientV2
    {
        private readonly Pusher _pusherClient;
        public delegate void OnStateChanged(ConnectionState state);
        public event OnStateChanged StateChanged;
        public delegate void OnPusherError(PusherException ex);
        public event OnPusherError PusherError;
        public KunaSocketClient( ) : base("KunaSocketClient",new KunaSocketClientOptions("wss://echo.websocket.org"))
        {
            _pusherClient = new Pusher("4b6a8b2c758be4e58868", new PusherOptions() {  Encrypted = true, Endpoint = "pusher.kuna.io", ProtocolNumber = 7, Version = "3.0.0" });
            _pusherClient.Connect();
            _pusherClient.Error += _pusherClient_Error;
            _pusherClient.ConnectionStateChanged += _pusherClient_ConnectionStateChanged;
        }
        public KunaSocketClient(KunaSocketClientOptions options):base("KunaSocketClient", options)
        {
            _pusherClient = new Pusher("4b6a8b2c758be4e58868", new PusherOptions() {  Encrypted = true, Endpoint = "pusher.kuna.io", ProtocolNumber = 7, Version = "3.0.0" });
            _pusherClient.Connect();
            _pusherClient.Error += _pusherClient_Error;
            _pusherClient.ConnectionStateChanged += _pusherClient_ConnectionStateChanged;
        }
       
        private void _pusherClient_ConnectionStateChanged(object sender, ConnectionState state)
        {
            log.Write(LogLevel.Debug, $"Pusher state is {state.ToString()}");
            StateChanged?.Invoke(state);
            switch (state)
            {
                case ConnectionState.Initialized:
                    break;
                case ConnectionState.Connecting:                 
                    break;
                case ConnectionState.Connected:
                    break;
                case ConnectionState.Disconnected:
                    break;
                case ConnectionState.WaitingToReconnect:
                    break;
                default:
                    break;
            }

        }

        private void _pusherClient_Error(object sender, PusherException error)
        {
            log.Write(LogLevel.Error, error.ToString());
            PusherError?.Invoke(error);
        }
        #region Channels
        /// <summary>
        /// need to send market, eg btcuah
        /// </summary>
        private const string MarketTradesChannel = "market-{}-global";

        #endregion
        public void SubscribeToOrderBookSideUpdatesV2(string market, Action<KunaOrderBookUpdateEventV2,string> onUpdate)
        {
            var _myChannel = _pusherClient.Subscribe(FillPathParameter(MarketTradesChannel, market));
            _myChannel.Bind("update", (dynamic data) =>
            {
                string t = Convert.ToString(data);          
                KunaOrderBookUpdateEventV2 deserialized = Deserialize<KunaOrderBookUpdateEventV2>(t).Data;
                onUpdate(deserialized,market);
            });

        }
        public void MyChannel()
        {
            var _myChannel = _pusherClient.Subscribe("private-7GBET5ZHZ");
            _myChannel.Bind("account", (dynamic data) =>
            {
                string t = Convert.ToString(data);
                Console.WriteLine(data);
            });

        }
        protected static string FillPathParameter(string path, params string[] values)
        {
            foreach (var value in values)
            {
                var index = path.IndexOf("{}", StringComparison.Ordinal);
                if (index >= 0)
                {
                    path = path.Remove(index, 2);
                    path = path.Insert(index, value);
                }
            }
            return path;
        }
        public void SubscribeToTradesV2(string market, Action<KunaTradeEventV2,string> onUpdate)
        {
            var _myChannel = _pusherClient.Subscribe(FillPathParameter(MarketTradesChannel, market));
            _myChannel.Bind("trades", (dynamic data) =>
            {
                string t = Convert.ToString(data);
                KunaTradeEventV2 deserialized = Deserialize<KunaTradeEventV2>(t).Data;
                onUpdate(deserialized, market);
            });

        }
        public override Task UnsubscribeAllAsync()
        {
            if(_pusherClient!=null)
            {
                _pusherClient.UnbindAll();
                _pusherClient.Disconnect();

            }
            return Task.CompletedTask;
        }
        public override void Dispose()
        {            
            base.Dispose();            
        }

        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
        {
            throw new NotImplementedException();
        }

        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken message, out CallResult<object> callResult)
        {
            throw new NotImplementedException();
        }

      

        protected override Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription s)
        {
            throw new NotImplementedException();
        }

        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            throw new NotImplementedException();
        }

        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            throw new NotImplementedException();
        }
    }

}
