using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Converters;
using Kuna.Net.Objects;
using Kuna.Net.Objects.V2;
using Microsoft.Extensions.Logging;
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
        private bool v3;
        public List<KunaOrderBookEntryV2> Ask = new List<KunaOrderBookEntryV2>();
        public List<KunaOrderBookEntryV2> Bid = new List<KunaOrderBookEntryV2>();

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public delegate void OrderBookUpdated();
       // public event OrderBookUpdated OnOrderBookUpdate;
        private CancellationTokenSource cancellationToken;
        public KunaSymbolOrderBook(string symbol, KunaSymbolOrderBookOptions options) : base($"Kuna-{symbol}",symbol, options)
        {
            _useSocketClient = false;
            _responseTimeout = options.ResponseTimeout;
            _orderBookLimit = options.EntriesCount;
            _timeOut = options.UpdateTimeout;
            httpClient = options.HttpClient?? new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_responseTimeout);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
            _orderBookLimit = options.EntriesCount;
            v3 = options.Usev3;
        }
        public KunaSymbolOrderBook(string symbol, KunaSocketClient socketClient, KunaSymbolOrderBookOptions options) : base($"Kuna-{symbol}",symbol, options)
        {
            v3 = options.Usev3;
            _useSocketClient = true;
            _responseTimeout = options.ResponseTimeout;
            _orderBookLimit = options.EntriesCount;
            _timeOut = options.UpdateTimeout;
            httpClient = options.HttpClient ?? new HttpClient();
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
            log.Write(LogLevel.Debug, "Infinite task was canceled with status:\n" + arg2.ToString());
        }

        private void Run()
        {
            LastUpdate = DateTime.UtcNow;
            if (_useSocketClient)
            {
                _kunaSocketClient.SubscribeToOrderBookSideUpdatesV2(this.Symbol, SocketOrderBookUpdate);
            }
            else
            {
                cancellationToken?.Dispose();
                cancellationToken = null;
                cancellationToken = new CancellationTokenSource();
                Task.Factory.StartNew(async () =>
                     await Watch(), cancellationToken.Token).ContinueWith(Catch, TaskContinuationOptions.OnlyOnFaulted);
            }

        }

        private void SocketOrderBookUpdate(KunaOrderBookUpdateEventV2 arg1, string arg2)
        {   
            SetInitialOrderBook(DateTime.UtcNow.Ticks, arg1.Bids.OrderByDescending(c => c.Price), arg1.Asks.OrderBy(c=>c.Price) );
        }
        
        public void StopGettingOrderBook()
        {
            cancellationToken.Cancel();
        }
        SemaphoreSlim _slim = new SemaphoreSlim(1);
        private async Task Watch()
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Dispose();
                    break;
                }
                if (v3)
                {
                    await GetOrderBookV3();
                }
                else
                {
                    await GetOrderBook();
                }
                await Task.Delay(_timeOut);
            }
        }


        private async Task<CallResult<bool>> GetOrderBook()
        {
            try
            {
                await _slim.WaitAsync().ConfigureAwait(false);             
                var result = await httpClient.GetAsync($"https://kuna.io/api/v2/depth?market={Symbol}&limit={_orderBookLimit}");
                if (result.IsSuccessStatusCode)
                {
                    var ob = result.Content.ReadAsStringAsync().Result;

                    var data = JsonConvert.DeserializeObject<KunaOrderBookV2>(ob);

                    SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Bids.OrderByDescending(c=>c.Price), data.Asks.OrderBy(c=>c.Price));

                    LastUpdate = DateTime.UtcNow;
                   // OnOrderBookUpdate?.Invoke();
                    _slim.Release();

                    return new CallResult<bool>(true);
                }
                else
                {
                    _slim.Release();

                    log.Write(LogLevel.Debug, $"Order book was not got");
                    return new CallResult<bool>( new KunaApiCallErrorV2((int)result.StatusCode, $"Order book was not got: {result.ReasonPhrase}"));
                }
            }
            catch (Exception ex)
            {
                log.Write(LogLevel.Error, $"Order book was not got cause\n{ex.ToString()}");
                _slim.Release();
                return new CallResult<bool>(new KunaApiCallErrorV2(-13, $"{ex.ToString()}"));
            }
        }

        private async Task<CallResult<bool>> GetOrderBookV3()
        {
            try
            {
                await _slim.WaitAsync().ConfigureAwait(false);     
                var result = await httpClient.GetAsync($"https://api.kuna.io/v3/book/{Symbol}");
                if (result.IsSuccessStatusCode)
                {
                    var ob = result.Content.ReadAsStringAsync().Result;

                    var data = JsonConvert.DeserializeObject<List<KunaOrderBookEntryV2>>(ob);
                    var asks = data.Where(c => c.Quantity < 0).Select(c => new KunaOrderBookEntryV2() { Quantity = c.Quantity * -1, Price = c.Price, Count = c.Count });
                    var bids = data.Where(c => c.Quantity > 0).Select(c => new KunaOrderBookEntryV2() { Quantity = c.Quantity, Price = c.Price, Count = c.Count });
                    Ask.Clear();
                    Ask.AddRange(asks.OrderBy(c => c.Price));
                    Bid.Clear();
                    Bid.AddRange(bids.OrderByDescending(c => c.Price));

                    SetInitialOrderBook(DateTime.UtcNow.Ticks, bids.OrderByDescending(c => c.Price), asks.OrderBy(c => c.Price));
                    LastUpdate = DateTime.UtcNow; 
                    _slim.Release();

                    return new CallResult<bool>(true);
                }
                else
                {
                    _slim.Release();

                    log.Write(LogLevel.Debug, $"Order book was not got");
                    return new CallResult<bool>( new KunaApiCallErrorV2((int)result.StatusCode, $"Order book was not got: {result.ReasonPhrase}"));
                }
            }
            catch (Exception ex)
            {
                log.Write(LogLevel.Error, $"Order book was not got cause\n{ex.ToString()}");
                _slim.Release();

                return new CallResult<bool>(new KunaApiCallErrorV2(-13, $"{ex.ToString()}"));
            }
        }
        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();
            httpClient?.Dispose();
        }

        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct=default)
        {
            // throw new NotImplementedException();

            return await GetOrderBook();
            // return new CallResult<bool>(true,null);
        }
        WebsocketFactory wf = new WebsocketFactory();
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct = default)
        {
            Run();
               
            return new CallResult<UpdateSubscription>(new UpdateSubscription(new FakeConnection(_kunaSocketClient, new KunaSocketApiClient(new FakeBaseOpts(),new FakeApiOpts()), wf.CreateWebsocket(log, "wss://echo.websocket.org")),null));
        }

    }
    internal class KunaSocketApiClient : SocketApiClient
    {
        public KunaSocketApiClient(BaseClientOptions options, ApiClientOptions apiOptions) : base(options, apiOptions)
        {
        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            return new KunaAuthenticationProvider(credentials);
        }
    }
    internal class FakeConnection : SocketConnection
    {
        public FakeConnection(BaseSocketClient client, SocketApiClient client1, IWebsocket socket) : base(client, client1, socket)
        {
        }
    }
    internal class FakeBaseOpts : BaseClientOptions
    {

    }
    internal class FakeApiOpts : ApiClientOptions
    {

    }
}
