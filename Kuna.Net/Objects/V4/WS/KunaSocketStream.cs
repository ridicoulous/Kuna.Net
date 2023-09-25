using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Options;
using CryptoExchange.Net.Sockets;
using Kuna.Net.Objects.V4.WS;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PureSocketCluster;
using PureWebSockets;

namespace Kuna.Net.Objects.V4;

public class KunaSocketStream : SocketApiClient
{
    private const string url = "wss://ws-pro.kuna.io/socketcluster/";
    private string jwt = null;

    internal ILogger Logger { get => _logger; }
    public KunaSocketStream(ILogger logger, SocketExchangeOptions options, SocketApiOptions apiOptions) : base(logger,url , options, apiOptions)
    {
    }


    /// <summary>
    /// The stream contains an array of KunaSocketTicker objects, each representing a different trading pair on the exchange.
    /// Update Speed: 1000ms
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToAllTickers(Action<IEnumerable<KunaSocketTicker>> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("arrTicker", onData, ct);
    }
    /// <summary>
    /// The stream contains an array of KunaMiniTicker objects, each representing a different trading pair on the exchange.
    /// Update Speed: 1000ms
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToAllMiniTickers(Action<IEnumerable<KunaMiniTicker>> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("arrMiniTicker", onData, ct);
    }
    /// <summary>
    ///  The real-time data stream that provides updates on the latest trading price and other market data for a specific trading pair.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToTicker(string symbol, Action<KunaSocketTicker> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal(AddSymbolToTopic(symbol, "ticker"), onData, ct);
    }
    /// <summary>
    ///  The real-time data stream that provides updates on the latest trading price and other market data for a specific trading pair.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToMiniTicker(string symbol, Action<KunaMiniTicker> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal(AddSymbolToTopic(symbol, "miniTicker"), onData, ct);
    }
    /// <summary>
    /// This is full order book depth stream.
    /// Update Speed: 1000ms
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToSlowOrderBook(string symbol, Action<KunaOrderBookV4> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal(AddSymbolToTopic(symbol, "depth"), onData, ct);
    }
    /// <summary>
    /// This is full order book depth stream.
    /// Update Speed: 100ms
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBook(string symbol, Action<KunaOrderBookV4> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal(AddSymbolToTopic(symbol, "depth100ms"), onData, ct);
    }

    /// <summary>
    ///  The stream provides real-time updates for changes to a user's order.
    ///  Each update includes information about the order, such as the order ID, 
    ///  the symbol, the order side (buy or sell), the order type (limit, market, etc.),
    ///  the order status (open, filled, cancelled, etc.), and the price and quantity of the order.
    /// Update Speed: real-time
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToOrdersUpdates(Action<KunaOrderV4> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("order", onData, ct);
    }

    /// <summary>
    ///  The trade stream provides real-time updates for new trades executed by a user. 
    ///  Each update includes information about the trade, such as the trade ID, the pair, 
    ///  the trade side (buy or sell), the price and quantity of the trade, the timestamp of the trade, etc.
    /// Update Speed: real-time
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToUserTrades(Action<KunaUserTradeV4> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("trade", onData, ct);
    }

    /// <summary>
    /// The stream provides real-time updates for changes to a user's account balances.
    /// Each update includes information about the account balance, such as the asset and the available and blocked balance on it.
    /// Update Speed: real-time
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToUserTrades(Action<SocketBalance> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("accounts", onData, ct);
    }

    /// <summary>
    /// The stream provides real-time updates for changes to a currencies rate. 
    /// Each update includes information about changes reference currencies (for now they are USD, UAH, BTC, EUR) for each asset.
    /// </summary>
    /// <param name="onData"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CallResult<UpdateSubscription>> SubscribeToCurenciesRates(Action<IEnumerable<KunaCurrencyRate>> onData, CancellationToken ct = default)
    {
        return await SubscribeInternal("rates", onData, ct);
    }

    internal async Task<CallResult<UpdateSubscription>> SubscribeInternal<TUpdate>(string topic, Action<TUpdate> onData, CancellationToken ct = default)
    where TUpdate : class
    {
        var req = new KunaSubscribeSocketRequest(topic);
        return await base.SubscribeAsync<KunaSocketSubscribtionIncomingUpdates<TUpdate>>
        (
            req,
            topic,
            authenticated: true,    //force to send auth payload
            dataHandler: updData => onData.Invoke(updData.Data.ActualData),
            ct: ct
        ).ConfigureAwait(false);
    }


    protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection socketConnection)
    {
        bool wasSuccessful = await SendHandshakeReq(socketConnection);

        if (jwt is null)
        {
            wasSuccessful = await SendLoginReq(socketConnection);
        }
        
        return wasSuccessful ? new CallResult<bool>(true) : new CallResult<bool>(new ServerError("Auth request was not succesful"));
    }

    private async Task<bool> SendLoginReq(SocketConnection socketConnection)
    {
        var isSuccess = false;
        var req = new KunaLoginToSocketRequest(ApiOptions.ApiCredentials?.Secret.GetString());
        await socketConnection.SendAndWaitAsync(req, TimeSpan.FromSeconds(1), null, 1, token =>
        {
            if (string.IsNullOrEmpty(token.ToString()))
            {
                isSuccess = false;
                return false;
            }
            jwt = token.ToObject<KunaBaseSocketData<TokenData>>()?.Data?.JWT;
            isSuccess = true;
            return true;

        });
        return isSuccess;

    }

    private async Task<bool> SendHandshakeReq(SocketConnection socketConnection)
    {
        var isSuccess = false;
        var authRequest = new KunaAuthSocketRequest(jwt);
        await socketConnection.SendAndWaitAsync(authRequest, TimeSpan.FromSeconds(1), null, 1, token =>
        {
            isSuccess = !string.IsNullOrEmpty(token.ToString());
            return true;

        });
        return isSuccess;
    }

    protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
    {
        return new KunaAuthenticationProvider(credentials);
    }

    protected override bool HandleQueryResponse<T>(SocketConnection socketConnection, object request, JToken data, [NotNullWhen(true)] out CallResult<T> callResult)
    {
        throw new System.NotImplementedException();
    }

    protected override bool HandleSubscriptionResponse(SocketConnection socketConnection, SocketSubscription subscription, object request, JToken data, out CallResult<object> callResult)
    {
        var response = Deserialize<KunaSocketResponse>(data);
        callResult = response.As<object>(response);
        return response.Data?.Id == ((KunaSubscribeSocketRequest)request).Id;
    }

    protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
    {
        if (!message.HasValues)
            return false;
        var response = Deserialize<KunaSocketSubscribtionIncomingUpdates<object>>(message);
        return response.Data?.Data?.Channel == ((KunaSubscribeSocketRequest)request).Data?.Channel;
    }

    protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
    {
        throw new System.NotImplementedException();
    }

    protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscriptionToUnsub)
    {
        var req = new KunaUnSubscribeSocketRequest(((KunaSubscribeSocketRequest) subscriptionToUnsub.Request).Data?.Channel);
        var success = false;

        await connection.SendAndWaitAsync(
            req,
            TimeSpan.FromSeconds(20),
            subscriptionToUnsub,
            1,
            data => 
                {
                    var response = Deserialize<KunaSocketResponse>(data);
                    if (response.Data?.Error is not null)
                    {
                        _logger.LogWarning(response.Data?.Error.Message);
                        return true;
                    }
                    success = true;
                    return true;
                });

        return success;
    }

    protected override async Task<CallResult<bool>> ConnectSocketAsync(SocketConnection socketConnection)
    {
        var result = await base.ConnectSocketAsync(socketConnection);
        if (result.Success)
            SendPeriodic("ping", TimeSpan.FromSeconds(8), connection => string.Empty);
        return result;
    }
    private string AddSymbolToTopic(string symbol, string topic)
    {
        return $"{symbol.ToLower()}@{topic}";
    }

}