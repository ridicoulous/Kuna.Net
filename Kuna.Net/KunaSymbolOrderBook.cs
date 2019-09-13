using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaSymbolOrderBook : SymbolOrderBook
    {
        private readonly bool _useSocketClient;
        private readonly KunaSocketClient _kunaSocketClient;
        private readonly HttpClient httpClient;
        private readonly int _orderBookLimit;
        private int _timeOut;
        private int _responseTimeout;


        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public delegate void OrderBookUpdated();
        public event OrderBookUpdated OnOrderBookUpdate;
        private CancellationTokenSource cancellationToken;
        public KunaSymbolOrderBook(string symbol, KunaSymbolOrderBookOptions options) : base(symbol, options)
        {
            _useSocketClient = false;
            _responseTimeout = options.ResponseTimeout;
            _orderBookLimit = options.EntriesCount;
            _timeOut = options.UpdateTimeout;
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_responseTimeout);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
            _orderBookLimit = options.EntriesCount;           
        }
        public KunaSymbolOrderBook(string symbol, KunaSocketClient socketClient, KunaSymbolOrderBookOptions options) : base(symbol, options)
        {
            _useSocketClient = true;
            _responseTimeout = options.ResponseTimeout;
            _orderBookLimit = options.EntriesCount;
            _timeOut = options.UpdateTimeout;
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_responseTimeout);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
            _orderBookLimit = options.EntriesCount;
            _kunaSocketClient = socketClient;
        }
        public void SetUpdateTimeout(int ms)
        {
            _timeOut = ms;
        }

        private void Catch(Task arg1, object arg2)
        {
            log.Write(CryptoExchange.Net.Logging.LogVerbosity.Debug, "Infinite task was canceled with status:\n" + arg2.ToString());
        }

        public void Run()
        {
            LastUpdate = DateTime.UtcNow;
            if (_useSocketClient)
            {              
                _kunaSocketClient.SubscribeToOrderBookSideUpdates(this.Symbol, SocketOrderBookUpdate);
            }
            else
            {
                cancellationToken?.Dispose();
                cancellationToken = null;
                cancellationToken = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                       Watch(), cancellationToken.Token).ContinueWith(Catch, TaskContinuationOptions.OnlyOnFaulted);
            }
            
        }

        private void SocketOrderBookUpdate(KunaOrderBookUpdateEvent arg1, string arg2)
        {
            var asks = arg1.Asks.Select(c => new OrderBookEntry(c.Price, c.Amount));
            var bids = arg1.Bids.Select(c => new OrderBookEntry(c.Price, c.Amount));
            SetInitialOrderBook(DateTime.UtcNow.Ticks, asks, bids);
            LastUpdate = DateTime.UtcNow;
            OnOrderBookUpdate?.Invoke();
        }

        public void StopGettingOrderBook()
        {
            cancellationToken.Cancel();
        }
        private void Watch()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Dispose();
                    break;
                }
                GetOrderBook();
                Thread.Sleep(_timeOut);
            }
        }


        private CallResult<bool> GetOrderBook()
        {
            try
            {
                var result = httpClient.GetAsync($"https://kuna.io/api/v2/depth?market={Symbol}&limit={_orderBookLimit}").Result;
                if (result.IsSuccessStatusCode)
                {
                    var ob = result.Content.ReadAsStringAsync().Result;

                    var data = JsonConvert.DeserializeObject<KunaOrderBook>(ob);

                    SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Asks, data.Bids);

                    LastUpdate = DateTime.UtcNow;
                    OnOrderBookUpdate?.Invoke();
                    return new CallResult<bool>(true, null);
                }
                else
                {
                    log.Write(CryptoExchange.Net.Logging.LogVerbosity.Debug, $"Order book was not getted");
                    return new CallResult<bool>(false, new KunaApiCallError((int)result.StatusCode, $"Order book was not getted: {result.ReasonPhrase}"));
                }
            }
            catch (Exception ex)
            {
                log.Write(CryptoExchange.Net.Logging.LogVerbosity.Error, $"Order book was not getted cause\n{ex.ToString()}");
                return new CallResult<bool>(false, new KunaApiCallError(-13, $"{ex.ToString()}"));
            }
        }

        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();
            httpClient?.Dispose();
        }

        protected override async Task<CallResult<bool>> DoResync()
        {
            // throw new NotImplementedException();

            //return GetOrderBook();
            return new CallResult<bool>(true,null);
        }
        
        protected override  async Task<CallResult<UpdateSubscription>> DoStart()
        {
            Run();
            var t = 
            //return null;
            //Status = OrderBookStatus.Syncing;
            //throw new NotImplementedException();
            // CallResult<UpdateSubscription> subResult = new CallResult<UpdateSubscription>(new UpdateSubscription(new SocketConnection(new So)));
            // return new CallResult<UpdateSubscription>(subResult.Data, null); 
            return new CallResult<UpdateSubscription>(new UpdateSubscription(new FakeConnection(), null), null);
        }

    }
    public class FakeConnection : SocketConnection
    {     

        public FakeConnection() : base(null, null)
        {
        }
    }
}
