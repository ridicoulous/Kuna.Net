using CryptoExchange.Net.Objects;
using Kuna.Net.Objects.V2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kuna.Net.Interfaces
{
    public interface IKunaClientV2
    {
        CallResult<DateTime> GetServerTimeV2();
        Task<CallResult<DateTime>> GetServerTimeV2Async(CancellationToken ct = default);

        CallResult<KunaTickerInfoV2> GetMarketInfoV2(string market);
        Task<CallResult<KunaTickerInfoV2>> GetMarketInfoV2Async(string market, CancellationToken ct = default);

        CallResult<KunaOrderBookV2> GetOrderBookV2(string market, int limit = 1000);
        Task<CallResult<KunaOrderBookV2>> GetOrderBookV2Async(string market, int limit = 1000, CancellationToken ct = default);

        CallResult<List<KunaTradeV2>> GetTradesV2(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");
        Task<CallResult<List<KunaTradeV2>>> GetTradesV2Async(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc", CancellationToken ct = default);

        CallResult<KunaAccountInfoV2> GetAccountInfoV2();
        Task<CallResult<KunaAccountInfoV2>> GetAccountInfoV2Async(CancellationToken ct = default);

        CallResult<KunaPlacedOrderV2> PlaceOrderV2(KunaOrderTypeV2 type, KunaOrderSideV2 side, decimal volume, decimal price, string market);
        Task<CallResult<KunaPlacedOrderV2>> PlaceOrderV2Async(KunaOrderTypeV2 type, KunaOrderSideV2 side, decimal volume, decimal price, string market, CancellationToken ct = default);

        CallResult<KunaPlacedOrderV2> CancelOrderV2(long orderId);
        Task<CallResult<KunaPlacedOrderV2>> CancelOrderV2Async(long orderId, CancellationToken ct = default);

        CallResult<List<KunaPlacedOrderV2>> GetMyOrdersV2(string market, KunaOrderStateV2 orderState = KunaOrderStateV2.Wait, int page = 1, string sort = "desc");
        Task<CallResult<List<KunaPlacedOrderV2>>> GetMyOrdersV2Async(string market, KunaOrderStateV2 orderState = KunaOrderStateV2.Wait, int page = 1, string sort = "desc", CancellationToken ct = default);

        CallResult<KunaPlacedOrderV2> GetOrderInfoV2(long orderId);
        Task<CallResult<KunaPlacedOrderV2>> GetOrderInfoV2Async(long orderId, CancellationToken ct = default);


        CallResult<List<KunaTradeV2>> GetMyTradesV2(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc");
        Task<CallResult<List<KunaTradeV2>>> GetMyTradesV2Async(string market, DateTime? fromDate = null, long? fromId = null, long? toId = null, int limit = 1000, string sort = "asc", CancellationToken ct = default);

        CallResult<List<KunaTraidingPairV2>> GeMarketsV2();

        Task<CallResult<List<KunaTraidingPairV2>>> GeMarketsV2Async(CancellationToken ct = default);
        CallResult<List<KunaOhclvV2>> GetCandlesHistoryV2(string symbol, int resolution, DateTime from, DateTime to);

        Task<CallResult<List<KunaOhclvV2>>> GetCandlesHistoryV2Async(string symbol, int resolution, DateTime from, DateTime to, CancellationToken token = default);

        CallResult<List<KunaCurrencyV2>> GetCurrenciesV2(CancellationToken ct = default);
        Task<CallResult<List<KunaCurrencyV2>>> GetCurrenciesV2Async(CancellationToken ct = default);



    }
}
