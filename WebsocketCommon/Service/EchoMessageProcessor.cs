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
        public Task<IWebSocketAnswer> ProcessClientMessage(Guid id, string inMessage)
        {
            var msg = JsonConvert.DeserializeObject<WebsocketMessage>(inMessage);
            if (msg?.SocketTopic == null)
                throw new ArgumentException($"{nameof(ProcessClientMessage)} received topic is null!");
            var answer = (IWebSocketAnswer)new WebSocketAnswer(msg.SocketTopic, new List<Guid> { id }, msg.Data, msg.Error);
            return Task.FromResult(answer);
        }
    }
}
