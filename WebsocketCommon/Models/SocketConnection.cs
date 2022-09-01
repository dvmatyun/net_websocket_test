using System;
using System.Net.WebSockets;

namespace WebsocketCommon.Models
{
    public class SocketConnection
    {
        public Guid Id { get; set; }
        public WebSocket WebSocket { get; set; }
        public DateTime ConnectionOpened { get; }

        public SocketConnection()
        {
            ConnectionOpened = DateTime.UtcNow;
        }
    }
}
