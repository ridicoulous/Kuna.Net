using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
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
    // private readonly PureSocketClusterSocket socket;
    // private readonly Dictionary<string,Channel> connections = new();
    private const string url = "wss://ws-pro.kuna.io/socketcluster/";
    public KunaSocketStream(ILogger logger, SocketExchangeOptions options, SocketApiOptions apiOptions) : base(logger,url , options, apiOptions)
    {
        // var pscOptions = new PureSocketClusterOptions()
        // {
        //     DebugMode = true,
        //     DisconnectWait = (int)options.RequestTimeout.TotalMilliseconds,
        //     MyReconnectStrategy = options.AutoReconnect ? new((int)options.ReconnectInterval.TotalMilliseconds) : new(0),
        //     Creds = options.ApiCredentials is null ? null : new() {ApiKey = options.ApiCredentials.Key.GetString(), ApiSecret = options.ApiCredentials.Secret.GetString() }
        // };
        // socket = new PureSocketClusterSocket(BaseAddress, pscOptions);
        // socket.OnData += OnData;
        // socket.OnMessage += OnMsg;
        // socket.On
    }

    private void OnData(object sender, byte[] data){
        Console.WriteLine($"{data} comes with {sender}");
    }
    private void OnMsg(object sender, string data)
    {
        Console.WriteLine($"{data} comes with {sender}");
    }

    public async Task<CallResult<UpdateSubscription>> SubscribeInternal<TUpdate>(string topic, Action<TUpdate> onData, CancellationToken ct = default)
    where TUpdate : class
    {
        var req = new KunaSubscribeSocketRequest(topic);
        // var req = new Dictionary<string, object>
        // {
        //     { "event", "#subscribe" },
        //     { "data", new Dictionary<string, string>() {{"channel", "arrTicker"}} },
        //     { "cid", 1 }
        // };

        return await base.SubscribeAsync<KunaSocketSubscribtionIncomingUpdates<TUpdate>>
        (
            req,
            topic,
            // authenticated: ApiOptions.ApiCredentials != null,
            authenticated: true,
            dataHandler: updData => onData.Invoke(updData.Data.ActualData),
            ct: ct
        );

        // topic = topic.ToLower();
        // while (socket.SocketState == WebSocketState.Connecting)
        // {
        //     await Task.Delay(1);
        // }
        // if (socket.SocketState != WebSocketState.Open && !await socket.ConnectAsync())
        // {
        //     _logger.LogError($"Can't connect to WebSocket");
        //     return;
        // }
        // if (connections.TryGetValue(topic, out var channel))
        // {
        //     await channel.UnsubscribeAsync();
        //     // connections.Remove(topic);
        // }
        // // socket.OnMessage += (sender, args) =>
        // channel = socket.CreateChannel(topic);
        // connections[topic] = channel;
        // await channel.SubscribeAsync();


        // await socket.SubscribeAsync(topic);
        // // channel = socket.CreateChannel(topic);
        // channel = socket.GetChannelByName(topic);
        // connections[topic] = channel;
        // await channel.SubscribeAsync();
    }


    protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection socketConnection)
    {
        // if (socketConnection.ApiClient.AuthenticationProvider is null)
        //     return new CallResult<bool>(false);

        ServerError serverError = null;
        bool isSuccess = false;
        var authRequest = new KunaAuthSocketRequest(ApiOptions.ApiCredentials?.Secret.GetString());
        await socketConnection.SendAndWaitAsync(authRequest, TimeSpan.FromSeconds(1), null, 1, token =>
        {
            if (string.IsNullOrEmpty(token.ToString()))
            {
                isSuccess = false;
                serverError = new ServerError("Auth request was not succesful");
            }
            else
            {
                isSuccess = true;
            }
            return true;

        });
        return isSuccess ? new CallResult<bool>(true) : new CallResult<bool>(serverError);
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

    // public override void Dispose()
    // {
    //     socket.Unsubscribe();
    //     base.Dispose();
    // }

}