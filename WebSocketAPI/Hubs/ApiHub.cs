using Microsoft.AspNetCore.SignalR;
using websocketapi.Services;

namespace websocketapi.Hubs
{
    public class ApiHub : Hub
    {
        private readonly WebSocketService _webSocketService;

        public ApiHub(WebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        public async Task SendMessageToClients()
        {
            // Отправляем последнее сообщение, полученное через WebSocket, всем подключенным клиентам
            await Clients.All.SendAsync("ReceiveMessage", _webSocketService.LastMessage);
        }

        public async Task GetLatestMessage()
        {
            // Отправляем последнее сообщение только вызывающему клиенту
            await Clients.Caller.SendAsync("ReceiveMessage", _webSocketService.LastMessage);
        }
    }
}
