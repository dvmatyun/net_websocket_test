using System;
using System.Collections.Generic;

namespace WebsocketCommon.Interface
{
    public interface IWebSocketAnswer : IWebsocketMessage
    {
        IEnumerable<Guid> ToWebsockets { get; set; }

        bool SendToAll { get; set; }
    }
}
