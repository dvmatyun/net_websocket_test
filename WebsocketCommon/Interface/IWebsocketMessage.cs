using System;
using System.Collections.Generic;

namespace WebsocketCommon.Interface
{

    /// <summary>
    /// Message to client
    /// </summary>
    public interface IWebsocketMessage
    {
        string Topic { get; set; }

        string Data { get; set; }

        string Error { get; set; }

    }
}
