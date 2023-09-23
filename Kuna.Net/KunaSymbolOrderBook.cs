using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kuna.Net.Interfaces;
using Kuna.Net.Helpers;
using Kuna.Net.Objects.V4;

namespace Kuna.Net
{
    public class KunaSymbolOrderBook : SymbolOrderBook, IDisposable
    {
        private const string id = "Kuna";
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10);
        private readonly KunaSocketStream _socketClient;
        private readonly KunaV4RestApiClient _restClient;
        private readonly bool isSocketClientOwner;
        private readonly bool isRestClientOwner;
        // private readonly TimeSpan _httpApiRefreshTimeout;
        // public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        // public delegate void OrderBookUpdated();
        // System.Threading.Timer apiBookChecker;
        // private readonly TimeSpan _checkTimeout;
        public KunaSymbolOrderBook(string symbol, ILogger logger) : base(logger, id, symbol)
        {
            _socketClient = new KunaSocketClient(new KunaSocketClientOptions()).MainSocketStreams;
            _restClient = (KunaV4RestApiClient)new KunaClient().ClientV4;
            isSocketClientOwner = true;
            isRestClientOwner = true;
        }
        public KunaSymbolOrderBook(string symbol, KunaSocketClient socketClient, KunaClient restClient) : base(socketClient?.Logger ?? restClient?.Logger, id, symbol)
        {
            _socketClient = socketClient?.MainSocketStreams;
            _restClient = (KunaV4RestApiClient) restClient?.ClientV4;
            if (_socketClient is null)
            {
                _socketClient = new KunaSocketClient(new KunaSocketClientOptions()).MainSocketStreams;
                isSocketClientOwner = true;
            }
            if (_restClient is null)
            {
                _restClient = (KunaV4RestApiClient)new KunaClient().ClientV4;
                isRestClientOwner = true;
            }
        }

        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct)
        {
            var book = await _restClient.GetOrderBookAsync(Symbol, ct: ct);
            if (!book)
                return new CallResult<bool>(book.Error!);

            SetInitialOrderBook(DateTime.UtcNow.Ticks, book.Data?.Bids, book.Data?.Asks);
            return new CallResult<bool>(true);
        }

        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            List<Task> tasks = new();
            //subscribe
            var subscrTask = _socketClient.SubscribeToOrderBook(Symbol, data => UpdateOrderBook(DateTime.UtcNow.Ticks, data.Bids, data.Asks), ct);
            tasks.Add(subscrTask);
            // set orderbook snapshot
            var restTask = _restClient.GetOrderBookAsync(Symbol, ct: ct);
            tasks.Add(restTask);
            var book = await restTask;
            if (!book.Success)
            {
                // LogLevel.Warning ? or LogLevel.Debug?
                _logger.Log(LogLevel.Warning, $"{Id} order book {Symbol} failed to retrieve initial order book");
                return new CallResult<UpdateSubscription>(book.Error);
            }
            SetInitialOrderBook(DateTime.UtcNow.Ticks, book.Data?.Bids, book.Data?.Asks);

            tasks.Add(WaitForSetOrderBookAsync(timeout, ct));
            await Task.WhenAll(tasks);
            return subscrTask.Result;
        }

        protected override void Dispose(bool disposing)
        {
            if (isSocketClientOwner)
            {
                _socketClient?.Dispose();
            }
            if (isRestClientOwner)
            {
                _restClient?.Dispose();
            }
            base.Dispose(disposing);
        }
        // private void GetBook(object state)
        // {
        //     Task.Run(GetOrderBookV3);
        // }
        // AsyncResetEvent? periodicEvent;
        // /// <summary>
        // /// The task that is sending periodic data on the websocket. Can be used for sending Ping messages every x seconds or similair. Not necesarry.
        // /// </summary>
        // protected Task? periodicTask;
        // /// <summary>
        // /// If client is disposing
        // /// </summary>
        // protected bool disposing;
        // /// <summary>
        // /// Dispose the client
        // /// </summary>
   
        // // public void SwitchUseApi()
        // // {
        // //     _shouldUseApi = !_shouldUseApi;
        // // }
        // // private bool _shouldUseApi;
        // public virtual void SendPeriodic()
        // {

        //     periodicEvent = new AsyncResetEvent();
        //     periodicTask = Task.Run(async () =>
        //     {
        //         while (!disposing)
        //         {
        //             await periodicEvent.WaitAsync(_httpApiRefreshTimeout).ConfigureAwait(false);
        //             if (disposing)
        //                 break;

        //             if (!_shouldUseApi)
        //                 continue;
        //             try
        //             {
        //                 await GetOrderBookV3();
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.Log(LogLevel.Warning, $"Periodic book {Symbol} failed: " + ex.ToLogString());
        //             }
        //         }
        //     });
        // }

        // private void Run()
        // {
        //     LastUpdate = DateTime.UtcNow;

        //     _kunaSocketClient?.SubscribeToOrderBookSideUpdatesV2(this.Symbol, SocketOrderBookUpdate);

        //     if (_kunaClient != null && _kunaSocketClient == null)
        //         SendPeriodic();

        // }

        // private void SocketOrderBookUpdate(KunaOrderBookUpdateEventV2 arg1, string arg2)
        // {
        //     LastUpdate = DateTime.UtcNow;
        //     if (asks.Values.Take(10).SequenceEqual(arg1.Asks.OrderBy(c => c.Price).Take(10), new OrderBookEntryComparer()) && bids.Values.Take(10).SequenceEqual(arg1.Bids.OrderByDescending(c => c.Price).Take(10), new OrderBookEntryComparer()))
        //     {
        //         return;
        //     }
        //     SetInitialOrderBook(DateTime.UtcNow.Ticks, arg1.Bids.OrderByDescending(c => c.Price), arg1.Asks.OrderBy(c => c.Price));
        //     apiBookChecker.Change((int)_checkTimeout.TotalMilliseconds, System.Threading.Timeout.Infinite);
        // }


        // private async Task GetOrderBookV3()
        // {
        //     try
        //     {

        //         var book = await _kunaClient?.ClientV3?.GetOrderBookAsync(Symbol);
        //         if (book && book != null)
        //         {
        //             if (asks.Values.Take(10).SequenceEqual(book.Data.Asks.Take(10), new OrderBookEntryComparer()) && bids.Values.Take(10).SequenceEqual(book.Data.Bids.Take(10), new OrderBookEntryComparer()))
        //             {
        //                 return;
        //             }
        //             SetInitialOrderBook(DateTime.UtcNow.Ticks, book.Data.Bids, book.Data.Asks);
        //             LastUpdate = DateTime.UtcNow;
        //             apiBookChecker.Change((int)_checkTimeout.TotalMilliseconds, System.Threading.Timeout.Infinite);
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.Log(LogLevel.Error, $"Order book was not got cause\n{ex.ToString()}");
        //     }
        // }
 //             LastUpdate = DateTime.UtcNow;
        //             apiBookChecker.Change((int)_checkTimeout.TotalMilliseconds, System.Threading.Timeout.Infinite);
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.Log(LogLevel.Error, $"Order book was not got cause\n{ex.ToString()}");
        //     }
        // }


        // protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct = default)
        // {
        //     await GetOrderBookV3();
        //     return new CallResult<bool>(true);
        // }
        // WebsocketFactory wf = new WebsocketFactory();
        // protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct = default)
        // {
        //     Run();

        //     return new CallResult<UpdateSubscription>(
        //         new UpdateSubscription(
        //             new FakeConnection(_kunaSocketClient,
        //                 new KunaSocketApiClient(new FakeBaseOpts(), new FakeApiOpts()),
        //                 wf.CreateWebsocket(_logger, new WebSocketParameters(new Uri("wss://echo.websocket.org"), false)),
        //                 Symbol), null));
        // }

    }
    // internal class KunaSocketApiClient : SocketApiClient
    // {
    //     public KunaSocketApiClient(BaseClientOptions options, ApiClientOptions apiOptions) : base(options, apiOptions)
    //     {
    //     }

    //     protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
    //     {
    //         return new KunaAuthenticationProvider(credentials);
    //     }
    // }
    // internal class FakeConnection : SocketConnection
    // {
    //     public FakeConnection(BaseSocketClient client, SocketApiClient client1, IWebsocket socket, string tag) : base(client, client1, socket, tag)
    //     {
    //     }
    // }
    // internal class FakeBaseOpts : BaseClientOptions
    // {

    // }
    // internal class FakeApiOpts : ApiClientOptions
    // {

    // }
}
