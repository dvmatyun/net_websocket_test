using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsocketCommon.Interface;
using WebsocketCommon.Models;

namespace WebsocketCommon.Service
{
    public class WebSocketHandlerTextBase : IWebSocketHandlerBase<string>
    {
        private IWebSocketServiceBase WebsocketService { get; }

        private IMessageProcessor<string, IWebSocketAnswer> MessageProcessor { get; }

        public WebSocketHandlerTextBase(IWebSocketServiceBase websocketService, IMessageProcessor<string, IWebSocketAnswer> messageProcessor)
        {
            WebsocketService = websocketService ?? throw new ArgumentNullException(nameof(websocketService));
            MessageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
        }

        /// <summary>
        /// Returns long-living task that ends when socket is closed
        /// </summary>
        /// <param name="id">Id of socket connection</param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public virtual async Task Handle(Guid id, WebSocket webSocket)
        {
            await WebsocketService.AddWebsocketConnectionAsync(id, webSocket);
            /// For debug purpose:
            _ = EmulateDisconnection(webSocket);
            ///
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await InternalMessageHandler(id, webSocket);
                }
                catch (Exception e)
                {
                    var errMsg = new WebSocketAnswer(e, id);
                    await WebsocketService.SendMessageToSocketsAsync(errMsg);
                }
            }
        }

        private async Task EmulateDisconnection(WebSocket webSocket)
        {
            await Task.Delay(6000);
            if (webSocket.State == WebSocketState.Open)
            {
                webSocket.Abort();
            }
        }

        protected async Task InternalMessageHandler(Guid id, WebSocket webSocket)
        {
            var text = await ReceiveClientMessage(webSocket);
            var message = await MessageProcessor.ProcessClientMessageAsync(id, text);
            if (message != null && message.Topic != null)
                await WebsocketService.SendMessageToSocketsAsync(message);
        }


        public virtual async Task<string> ReceiveClientMessage(WebSocket webSocket)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);
            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                return message;
            }
            throw new ArgumentException($"{nameof(ReceiveClientMessage)} received message is not text!");
        }
    }
}
