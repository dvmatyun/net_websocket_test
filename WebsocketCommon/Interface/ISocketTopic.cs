using System.Collections.Generic;

namespace WebsocketCommon.Interface
{
    public interface ISocketTopic
    {
        string Topic { get; set; }

        IList<string> PathSegments { get; set; }
    }
}
