using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebsocketCommon.Interface;

namespace WebsocketCommon.Models
{
    public class WebSocketAnswer : WebsocketMessage, IWebSocketAnswer
    {
        public WebSocketAnswer()
        {
            SocketTopic = new SocketTopic("empty/empty");
        }

        public WebSocketAnswer(IList<string> pathSegments, IEnumerable<Guid> toWebsockets, object data, string error = null)
            : this(new SocketTopic(pathSegments), toWebsockets, data, error)
        {
        }

        public WebSocketAnswer(string topic, IEnumerable<Guid> toWebsockets, object data, string error = null)
            : this(new SocketTopic(topic), toWebsockets, data, error)
        {
        }

        public WebSocketAnswer(ISocketTopic socketTopic, IEnumerable<Guid> toWebsockets, object data, string error = null)
        {
            SocketTopic = socketTopic;
            ToWebsockets = new HashSet<Guid>(toWebsockets);
            Error = error;
            SerializeObjectToString(data);
        }

        public WebSocketAnswer(Exception error, Guid toSocket)
        {
            ToWebsockets = new HashSet<Guid> { toSocket };
            SocketTopic = new SocketTopic("error");
            Error = error.Message;
        }

        private ISet<Guid> _toPlayers;

        [JsonIgnore]
        public ISet<Guid> ToWebsockets
        {
            get { return _toPlayers ??= new HashSet<Guid>(); }
            set => _toPlayers = new HashSet<Guid>(value);
        }

        [JsonIgnore]
        public bool SendToAll { get; set; } = false;

        public void SerializeObjectToString<TY>(TY messageObj)
        {
            if (messageObj is string)
            {
                Data = messageObj as string;
            }
            else
            {
                Data = JsonConvert.SerializeObject(messageObj);
            }
            
        }

        public string GetAnswerToUser()
        {
            if (Topic == null)
            {
                throw new ArgumentException($"{nameof(WebSocketAnswer)} Topic must be assigned!");
            }
            var answer = new WebsocketMessage(this);
            return JsonConvert.SerializeObject(answer);
        }
        
    }
    

}


