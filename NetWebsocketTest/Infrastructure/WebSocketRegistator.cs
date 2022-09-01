using WebsocketCommon.Interface;
using WebsocketCommon.Service;

namespace NetWebsocketTest.Infrastructure
{
    public static class WebSocketRegistator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IWebSocketServiceBase, WebSocketServiceBase>();
            services.AddSingleton<IWebSocketHandlerBase<string>, WebSocketHandlerTextBase>();
            services.AddSingleton<IMessageProcessor<string, IWebSocketAnswer>, EchoMessageProcessor>();
        }
    }
}
