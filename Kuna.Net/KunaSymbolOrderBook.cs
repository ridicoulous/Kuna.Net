using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            _restClient = (KunaV4RestApiClient)restClient?.ClientV4;
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
    }
}
