using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsocketCommon.Interface;
using WebsocketCommon.Models;

namespace WebsocketCommon.Service
{
    public class WebSocketServiceBase : IWebSocketServiceBase
    {
        protected ConcurrentDictionary<Guid, SocketConnection> WebsocketConnections { get; }
        private CancellationTokenSource TokenSource { get; }

        public WebSocketServiceBase()
        {
            WebsocketConnections = new ConcurrentDictionary<Guid, SocketConnection>();
            TokenSource = new CancellationTokenSource();
            _ = SetupWebsocketCleanUpTask(TokenSource.Token);
        }


        /// <summary>
        /// Get websocket using context:
        /// using var websocket = await context.WebSockets.AcceptWebSocketAsync();
        /// </summary>
        /// <param name="id">Id of socket connection</param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public virtual async Task AddWebsocketConnectionAsync(Guid id, WebSocket webSocket, CancellationToken cancellationToken = default)
        {
            var isFound = WebsocketConnections.TryGetValue(id, out var connection);
            if (isFound && connection.WebSocket.State == WebSocketState.Open)
            {
                await connection.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        $"Another socket with id={id} connected!", cancellationToken);
            }

            WebsocketConnections.AddOrUpdate(id, addValueFactory: (guid => new SocketConnection
            {
                Id = id,
                WebSocket = webSocket
            }), updateValueFactory: ((_, __) => new SocketConnection
            {
                Id = id,
                WebSocket = webSocket
            }));

        }

        public virtual async Task SendDataToSocketsBaseAsync(IWebSocketAnswer webSocketAnswer, CancellationToken cancellationToken = default)
        {
            if (webSocketAnswer.Topic == null)
                throw new ArgumentException($"{nameof(WebsocketMessage)} SocketTopic must be assigned!");

            var serializedMessage = JsonConvert.SerializeObject(webSocketAnswer);
            var bytes = Encoding.Default.GetBytes(serializedMessage);
            var arraySegment = new ArraySegment<byte>(bytes);

            var tasks = webSocketAnswer.ToWebsockets.Select(async id =>
            {
                var isFound = WebsocketConnections.TryGetValue(id, out var connection);
                if (isFound && connection.WebSocket.State == WebSocketState.Open)
                {
                    await connection.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, cancellationToken);
                }
            }).ToList();
            await Task.WhenAll(tasks);
        }

        public async Task SendDataToSocketsAsync<T>(IEnumerable<Guid> toSockets, ISocketTopic topic, T data, string error = null, CancellationToken cancellationToken = default)
        {
            var wsMessage = new WebSocketAnswer(topic, toSockets, data, error);
            await SendDataToSocketsBaseAsync(wsMessage, cancellationToken);
        }

        public async Task SendDataToSocketsAsync<T>(IEnumerable<Guid> toSockets, string topic, T data, string error = null, CancellationToken cancellationToken = default)
        {
            var wsMessage = new WebSocketAnswer(topic, toSockets, data, error);
            await SendDataToSocketsBaseAsync(wsMessage, cancellationToken);
        }

        public virtual async Task SendMessageToSocketsAsync(IWebSocketAnswer message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                return;

            IEnumerable<Guid> toSentTo;
            if (message.SendToAll)
                toSentTo = WebsocketConnections.Keys.ToArray();
            else
                toSentTo = message.ToWebsockets;
            await SendDataToSocketsAsync(toSentTo, message.Topic, message.Data, message.Error, cancellationToken);
        }

        private Task SetupWebsocketCleanUpTask(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var socketIds = WebsocketConnections.Keys.ToArray();
                    foreach (var id in socketIds)
                    {
                        var isFound = WebsocketConnections.TryGetValue(id, out var socket);

                        if (!isFound)
                            continue;

                        if (socket.WebSocket.State != WebSocketState.Open && socket.WebSocket.State != WebSocketState.Connecting)
                        {
                            WebsocketConnections.TryRemove(socket.Id, out var closedConnection);
                        }
                    }
                    await Task.Delay(5000);
                }

            }, cancellationToken);
        }

        public void Dispose()
        {
            TokenSource.Cancel();
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            TokenSource.Cancel();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
