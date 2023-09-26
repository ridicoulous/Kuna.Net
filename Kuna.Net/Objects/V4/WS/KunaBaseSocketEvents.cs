using Newtonsoft.Json;

namespace Kuna.Net.Objects.V4.WS
{
    internal class KunaBaseSocketData<T>
    {
        [JsonProperty("event")]
        public string Topic { get; set; }
        
        [JsonProperty("data")]
        public T Data { get; set; }

    }

    internal class KunaSocketRequest<T> : KunaBaseSocketData<T>
    {
        private int? id;

        [JsonProperty("cid")]
        public int Id
        {
            get
            {
                id ??= CidManager.GetNextId();
                return id.Value;
            }
        }
    }
    internal static class CidManager
    {
        private static int latestId = 0;

        public static int GetNextId()
        {
            return ++latestId;
        }
    }

    internal class KunaSubscribeSocketRequest : KunaSocketRequest<KunaBaseSocketEventBody>
    {
        public KunaSubscribeSocketRequest(string channel)
        {
            Topic = "#subscribe";
            Data = new KunaBaseSocketEventBody(channel);
        }
    }
    internal class KunaAuthSocketRequest : KunaSocketRequest<KunaAuthSocketRequestBody>
    {
        public KunaAuthSocketRequest(string authToken)
        {
            Topic = "#handshake";
            Data = new KunaAuthSocketRequestBody(){AuthToken = authToken};
        }
    }
    internal class KunaAuthSocketRequestBody
    {
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }
    }
    internal class KunaUnSubscribeSocketRequest : KunaSocketRequest<string>
    {
        public KunaUnSubscribeSocketRequest(string channel)
        {
            Topic = "#unsubscribe";
            Data = channel;
        }
    }
    internal class KunaLoginToSocketRequest : KunaBaseSocketData<string>
    {
        public KunaLoginToSocketRequest(string apiKey)
        {
            Topic = "login";
            Data = apiKey;
        }
    }
    internal class KunaBaseSocketEventBody
    {
        public KunaBaseSocketEventBody()
        {
        }

        public KunaBaseSocketEventBody(string channel)
        {
            Channel = channel;
        }

        [JsonProperty("channel")]

        public string Channel { get; set; }
    }
    internal class KunaSubUpdateSocketEventBody<T> : KunaBaseSocketEventBody
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
    
    internal class KunaSocketSubscribtionIncomingUpdates<T> : KunaBaseSocketData<KunaSubUpdateSocketEventBody<KunaBaseSocketData<T>>>
    where T : class
    {
        /// <summary>
        /// the data inside billion wrappers
        /// </summary>
        public T ActualData => Data?.Data?.Data;
    }

    internal class KunaSocketResponse
    {
        [JsonProperty("rid")]
        public int? Id { get; set; }
        [JsonProperty("error")]
        public Error Error { get; set; }
    }

    internal class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    internal class TokenData
    {
        [JsonProperty("token")]
        public string JWT { get; set; }
    }
}