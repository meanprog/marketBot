using websocketapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрация WebSocketService
builder.Services.AddSingleton<WebSocketService>();

var app = builder.Build();

app.UseRouting();

// Определение эндпоинта API для клиента
app.MapGet("/api/data", (WebSocketService webSocketService) =>
{
    // Возвращаем последние данные, полученные через WebSocket
    return Results.Ok(new { message = webSocketService.LastMessage });
});

// Запуск WebSocket-сервиса для подключения к market.csgo.com
var webSocketService = app.Services.GetRequiredService<WebSocketService>();
_ = webSocketService.StartWebSocketLoop(); // Запускаем метод без ожидания

app.Run();
