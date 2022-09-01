using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsocketCommon.Interface;

namespace NetWebsocketTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private IWebSocketHandlerBase<string> WebsocketHandler { get; }

        // Register WebSocketHandlerTextBase first in DI!!!
        public WebSocketController(IWebSocketHandlerBase<string> websocketHandler)
        {
            WebsocketHandler = websocketHandler;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task GetWebsocketConnection()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (!isSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var websocket = await context.WebSockets.AcceptWebSocketAsync();
            await WebsocketHandler.Handle(Guid.NewGuid(), websocket);
        }
    }
}