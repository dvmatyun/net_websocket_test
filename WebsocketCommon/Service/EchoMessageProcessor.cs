using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebsocketCommon.Interface;
using WebsocketCommon.Models;

namespace WebsocketCommon.Service
{
    public class EchoMessageProcessor : IMessageProcessor<string, IWebSocketAnswer>
    {
        private ISocketTopic PongTopic = new SocketTopic("pong");

        public IWebSocketAnswer PingMessageGetResponseOrNull(Guid id, string inMessage)
        {
            if (inMessage == "ping")
                return new WebSocketAnswer(PongTopic, id, null);

            return null;
        }

        public Task<IWebSocketAnswer> ProcessClientMessageAsync(Guid id, string inMessage)
        {
            var pong = PingMessageGetResponseOrNull(id, inMessage);
            if (pong != null)
                return Task.FromResult(pong);

            var msg = JsonConvert.DeserializeObject<WebsocketMessage>(inMessage);
            if (msg?.SocketTopic == null)
                throw new ArgumentException($"{nameof(ProcessClientMessageAsync)} received topic is null!");
            var answer = (IWebSocketAnswer)new WebSocketAnswer(msg.SocketTopic, id, msg.Data, msg.Error);
            return Task.FromResult(answer);
        }
    }
}
