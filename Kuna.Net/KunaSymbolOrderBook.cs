using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net
{
    public class KunaSymbolOrderBook : SymbolOrderBook
    {
        HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(2) };
        private readonly int _orderBookLimit;
        private int _timeOut;

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public delegate void OrderBookUpdated();
        public event OrderBookUpdated OnOrderBookUpdate;
        private CancellationTokenSource cancellationToken;
        public KunaSymbolOrderBook(string symbol, KunaSymbolOrderBookOptions options) : base(symbol, options)
        {
            _orderBookLimit = options.EntriesCount ?? 1000;
            _timeOut = options.UpdateTimeout ?? 300;
            //  new Thread(Watch).Start();

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
            cancellationToken?.Dispose();
            cancellationToken = null;
            cancellationToken = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
                   Watch(), cancellationToken.Token).ContinueWith(Catch, TaskContinuationOptions.OnlyOnFaulted);
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
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private CallResult<bool> GetOrderBook()
        {
            try
            {               
                var result = httpClient.GetAsync($"https://kuna.io/api/v2/depth?market={Symbol}&limit={_orderBookLimit}").Result;
                if (result.IsSuccessStatusCode)
                {
                    var ob = result.Content.ReadAsStringAsync().Result;
                    
                    var data = JsonConvert.DeserializeObject<KunaOrderBook>(ob);
                    SetInitialOrderBook((long)DateTime.UtcNow.Subtract(unixEpoch).TotalMilliseconds, data.Asks, data.Bids);
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

        protected override Task<CallResult<bool>> DoResync()
        {
            throw new NotImplementedException();

            //return GetOrderBook();
        }

        protected override Task<CallResult<UpdateSubscription>> DoStart()
        {
            //Status = OrderBookStatus.Syncing;
            throw new NotImplementedException();
            //return new CallResult<UpdateSubscription>(new UpdateSubscription(new SocketConnection(new KunaSocketClient()),new SocketSubscription()),null)
        }

    }
}
