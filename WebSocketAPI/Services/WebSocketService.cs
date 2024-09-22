using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace websocketapi.Services
{
    public class WebSocketService
    {
        private string _lastMessage;
        public string LastMessage => _lastMessage;
        private string _token = "";

        public async Task StartWebSocketLoop()
        {
            while (true)
            {
                try
                {
                    _token = await GetWsToken();
                    await ConnectToWebSocket(_token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(9));
            }
        }

        private async Task<string> GetWsToken()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://market.csgo.com/api/v2/get-ws-token?key=A66x6fE01Kurf11LMNkoZZItV1tMhie");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(jsonResponse)["token"]?.ToString();
            Console.WriteLine($"Received token: {token}");
            return token;
        }

        private async Task ConnectToWebSocket(string token)
        {
            using var clientWebSocket = new ClientWebSocket();
            var uri = new Uri("wss://wsprice.csgo.com/connection/websocket");

            try
            {
                Console.WriteLine($"Connecting to WebSocket at {uri} with token {token}...");
                await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Console.WriteLine("Connected to WebSocket.");

                var subscriptionMessage = new
                {
                    method = "subscribe",
                    @params = new
                    {
                        channel = "public:items:730:rub",
                        token = token
                    }
                };

                var messageJson = JsonConvert.SerializeObject(subscriptionMessage);
                var messageBytes = Encoding.UTF8.GetBytes(messageJson);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Subscription message sent.");

                await ReceiveMessages(clientWebSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message}");
            }
        }

        private async Task ReceiveMessages(ClientWebSocket clientWebSocket)
        {
            var buffer = new byte[1024 * 4];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {receivedMessage}");
                    var jsonMessage = JObject.Parse(receivedMessage);
                    _lastMessage = jsonMessage.ToString();
                    OnMessageReceived(jsonMessage);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"WebSocket connection closed with status: {result.CloseStatus}, description: {result.CloseStatusDescription}");
                    break;
                }
            }
        }

        private void OnMessageReceived(JObject message)
        {
            // Обработка полученного сообщения
            // Реализуйте логику для обработки различных типов сообщений
            Console.WriteLine("Message received: " + message.ToString());
        }

        public async Task Unsubscribe(ClientWebSocket clientWebSocket)
        {
            var unsubscribeMessage = new
            {
                method = "unsubscribe",
                @params = new
                {
                    channel = "public:items:730:rub",
                    token = _token
                }
            };

            var messageJson = JsonConvert.SerializeObject(unsubscribeMessage);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Unsubscription message sent.");
        }
    }
}
