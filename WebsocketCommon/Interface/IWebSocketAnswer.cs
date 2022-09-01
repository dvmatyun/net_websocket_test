using System;
using System.Collections.Generic;

namespace WebsocketCommon.Interface
{
    public interface IWebSocketAnswer : IWebsocketMessage
    {
        ISet<Guid> ToWebsockets { get; set; }

        bool SendToAll { get; set; }
    }
}
