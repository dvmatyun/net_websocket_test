using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebsocketCommon.Interface
{
    public interface IWebSocketHandlerBase<T>
    {
        /// <summary>
        /// Returns long-living task that ends when socket is closed
        /// </summary>
        /// <param name="id">Id of socket connection</param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task Handle(Guid id, WebSocket webSocket);

        /// <summary>
        /// This method listenes to client messages and deserializes them to generic type
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task<T> ReceiveClientMessage(WebSocket webSocket);

    }
}
