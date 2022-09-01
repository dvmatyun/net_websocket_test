using Newtonsoft.Json;
using System;
using WebsocketCommon.Interface;

namespace WebsocketCommon.Models
{
    /// <summary>
    /// Message to client
    /// </summary>
    public class WebsocketMessage : IWebsocketMessage
    {
        public WebsocketMessage()
        {
        }

        [JsonIgnore]
        public ISocketTopic SocketTopic { get; set; }

        public string Topic { get => SocketTopic.Topic; set => SocketTopic = new SocketTopic(value); }

        public string Data { get; set; }

        public string Error { get; set; }

        public WebsocketMessage(string topic, object data, string error) : this(new SocketTopic(topic), data, error)
        {
        }

        public WebsocketMessage(SocketTopic socketTopic, object data, string error)
        {
            SocketTopic = socketTopic;
            if (data != null)
                Data = JsonConvert.SerializeObject(data);
            Error = error;
        }

        public string GetSerializedMessage()
        {
            if (Topic == null)
                throw new ArgumentException($"{nameof(WebsocketMessage)} SocketTopic must be assigned!");

            return JsonConvert.SerializeObject(this);
        }

        public WebsocketMessage(IWebsocketMessage websocketMessage)
        {
            Data = websocketMessage.Data;
            Topic = websocketMessage.Topic;
            Error = websocketMessage.Error;
        }
        
    }
}
