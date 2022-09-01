using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebsocketCommon.Interface
{
    public interface IWebSocketServiceBase : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Get websocket using context:
        /// using var websocket = await context.WebSockets.AcceptWebSocketAsync();
        /// </summary>
        /// <param name="id"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        Task AddWebsocketConnectionAsync(Guid id, WebSocket webSocket, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send message to list of sockets
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToSocketsAsync(IWebSocketAnswer message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send data to list of sockets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSockets"></param>
        /// <param name="topic"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Task SendDataToSocketsAsync<T>(IEnumerable<Guid> toSockets, string topic, T data, string error = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send data to list of sockets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSockets"></param>
        /// <param name="topic"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        Task SendDataToSocketsAsync<T>(IEnumerable<Guid> toSockets, ISocketTopic topic, T data, string error = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send data to list of sockets. Override this method in order to override logic of SendDataToSocketsAsync.
        /// </summary>
        /// <param name="webSocketAnswer"></param>
        /// <returns></returns>
        Task SendDataToSocketsBaseAsync(IWebSocketAnswer webSocketAnswer, CancellationToken cancellationToken = default);

    }
}
