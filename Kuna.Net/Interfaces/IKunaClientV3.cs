using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.ExchangeInterfaces;
using CryptoExchange.Net.Objects;
using Kuna.Net.Objects.V3;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClientV3 : IExchangeClient
    {
        Task<CallResult<List<KunaPlacedOrder>>> GetOrdersAsync(OrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null, CancellationToken ct = default);
        CallResult<List<KunaPlacedOrder>> GetOrders(OrderState state, string market = null, DateTime? from = null, DateTime? to = null, int? limit = null, bool? sortDesc = null);
        CallResult<List<KunaTrade>> GetOrderTrades(string market, long id);
        Task<CallResult<List<KunaTrade>>> GetOrderTradesAsync(string market, long id, CancellationToken ct);
    }
}