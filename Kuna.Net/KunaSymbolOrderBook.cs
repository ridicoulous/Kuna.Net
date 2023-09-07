// using CryptoExchange.Net;
// using CryptoExchange.Net.Authentication;
// using CryptoExchange.Net.Interfaces;
// using CryptoExchange.Net.Objects;
// using CryptoExchange.Net.OrderBook;
// using CryptoExchange.Net.Sockets;
// using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;
// using Kuna.Net.Interfaces;
// using Kuna.Net.Helpers;
// using System.Reactive;
// using System.Reactive.Linq;

// namespace Kuna.Net
// {
//     public class KunaSymbolOrderBook : SymbolOrderBook, IDisposable
//     {
//         private readonly KunaSocketClient _kunaSocketClient;
//         private readonly IKunaClient _kunaClient;
//         private readonly TimeSpan _httpApiRefreshTimeout;
//         public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
//         public delegate void OrderBookUpdated();
//         System.Threading.Timer apiBookChecker;
//         private readonly TimeSpan _checkTimeout;
//         public KunaSymbolOrderBook(string symbol, KunaSymbolOrderBookOptions options) : base($"Kuna-{symbol}", symbol, options)
//         {
//             _checkTimeout = options.CheckBookTimeout;
//             apiBookChecker = new Timer(GetBook, null, (int)options.CheckBookTimeout.TotalMilliseconds, -1 );
//             _shouldUseApi = options.KunaClient != null;
//             _kunaSocketClient = options.KunaSocketClient;
//             _kunaClient = options.KunaClient;
//             _httpApiRefreshTimeout = options.UpdateTimeout;
//         }

//         private void GetBook(object state)
//         {
//             Task.Run(GetOrderBookV3);
//         }
//         AsyncResetEvent? periodicEvent;
//         /// <summary>
//         /// The task that is sending periodic data on the websocket. Can be used for sending Ping messages every x seconds or similair. Not necesarry.
//         /// </summary>
//         protected Task? periodicTask;
//         /// <summary>
//         /// If client is disposing
//         /// </summary>
//         protected bool disposing;
//         /// <summary>
//         /// Dispose the client
//         /// </summary>
//         public void Dispose()
//         {
//             disposing = true;
//             periodicEvent?.Set();
//             periodicEvent?.Dispose();
//             log.Write(LogLevel.Debug, "Disposing socket client, closing all subscriptions");

//             base.Dispose();
//         }
//         public void SwitchUseApi()
//         {
//             _shouldUseApi = !_shouldUseApi;
//         }
//         private bool _shouldUseApi;
//         public virtual void SendPeriodic()
//         {

//             periodicEvent = new AsyncResetEvent();
//             periodicTask = Task.Run(async () =>
//             {
//                 while (!disposing)
//                 {
//                     await periodicEvent.WaitAsync(_httpApiRefreshTimeout).ConfigureAwait(false);
//                     if (disposing)
//                         break;

//                     if (!_shouldUseApi)
//                         continue;
//                     try
//                     {
//                         await GetOrderBookV3();
//                     }
//                     catch (Exception ex)
//                     {
//                         log.Write(LogLevel.Warning, $"Periodic book {Symbol} failed: " + ex.ToLogString());
//                     }
//                 }
//             });
//         }

//         private void Run()
//         {
//             LastUpdate = DateTime.UtcNow;

//             _kunaSocketClient?.SubscribeToOrderBookSideUpdatesV2(this.Symbol, SocketOrderBookUpdate);

//             if (_kunaClient != null && _kunaSocketClient == null)
//                 SendPeriodic();

//         }

//         private void SocketOrderBookUpdate(KunaOrderBookUpdateEventV2 arg1, string arg2)
//         {
//             LastUpdate = DateTime.UtcNow;
//             if (asks.Values.Take(10).SequenceEqual(arg1.Asks.OrderBy(c => c.Price).Take(10), new OrderBookEntryComparer()) && bids.Values.Take(10).SequenceEqual(arg1.Bids.OrderByDescending(c => c.Price).Take(10), new OrderBookEntryComparer()))
//             {
//                 return;
//             }
//             SetInitialOrderBook(DateTime.UtcNow.Ticks, arg1.Bids.OrderByDescending(c => c.Price), arg1.Asks.OrderBy(c => c.Price));
//             apiBookChecker.Change((int)_checkTimeout.TotalMilliseconds, System.Threading.Timeout.Infinite);
//         }


//         private async Task GetOrderBookV3()
//         {
//             try
//             {

//                 var book = await _kunaClient?.ClientV3?.GetOrderBookAsync(Symbol);
//                 if (book && book != null)
//                 {
//                     if (asks.Values.Take(10).SequenceEqual(book.Data.Asks.Take(10), new OrderBookEntryComparer()) && bids.Values.Take(10).SequenceEqual(book.Data.Bids.Take(10), new OrderBookEntryComparer()))
//                     {
//                         return;
//                     }
//                     SetInitialOrderBook(DateTime.UtcNow.Ticks, book.Data.Bids, book.Data.Asks);
//                     LastUpdate = DateTime.UtcNow;
//                     apiBookChecker.Change((int)_checkTimeout.TotalMilliseconds, System.Threading.Timeout.Infinite);
//                 }

//             }
//             catch (Exception ex)
//             {
//                 log.Write(LogLevel.Error, $"Order book was not got cause\n{ex.ToString()}");
//             }
//         }
       

//         protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct = default)
//         {
//             await GetOrderBookV3();
//             return new CallResult<bool>(true);
//         }
//         WebsocketFactory wf = new WebsocketFactory();
//         protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct = default)
//         {
//             Run();

//             return new CallResult<UpdateSubscription>(
//                 new UpdateSubscription(
//                     new FakeConnection(_kunaSocketClient,
//                         new KunaSocketApiClient(new FakeBaseOpts(), new FakeApiOpts()),
//                         wf.CreateWebsocket(log, new WebSocketParameters(new Uri("wss://echo.websocket.org"), false)),
//                         Symbol), null));
//         }

//     }
//     internal class KunaSocketApiClient : SocketApiClient
//     {
//         public KunaSocketApiClient(BaseClientOptions options, ApiClientOptions apiOptions) : base(options, apiOptions)
//         {
//         }

//         protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
//         {
//             return new KunaAuthenticationProvider(credentials);
//         }
//     }
//     internal class FakeConnection : SocketConnection
//     {
//         public FakeConnection(BaseSocketClient client, SocketApiClient client1, IWebsocket socket, string tag) : base(client, client1, socket, tag)
//         {
//         }
//     }
//     internal class FakeBaseOpts : BaseClientOptions
//     {

//     }
//     internal class FakeApiOpts : ApiClientOptions
//     {

//     }
// }
