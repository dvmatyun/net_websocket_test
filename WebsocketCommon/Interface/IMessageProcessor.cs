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
        Task<Y> ProcessClientMessage(Guid id, T inMessage);
    }
}
