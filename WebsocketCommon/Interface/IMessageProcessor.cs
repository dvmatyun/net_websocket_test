using System;
using System.Threading.Tasks;

namespace WebsocketCommon.Interface
{
    public interface IMessageProcessor<T, Y>
    {
        /// <summary>
        /// Listen to client's message and form an answer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inMessage"></param>
        /// <returns></returns>
        Task<Y> ProcessClientMessageAsync(Guid id, T inMessage);

        /// <summary>
        /// Checks whether the incoming message is connection pinging.
        /// If it is, then return NOT NULL simple pong answer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inMessage"></param>
        /// <returns></returns>
        Y PingMessageGetResponseOrNull(Guid id, T inMessage);
    }
}
