![image](https://github.com/user-attachments/assets/2c748e1f-9319-4216-a185-35bcbdd0b031)
закрывается подключение потому что как я понимаю (может, это не так ) неправильно подписываюсь на канал. 
```javascript
ws.on('open', () => {
            console.log('Connected to WebSocket');
            // Подписка на канал
            ws.send(JSON.stringify({
                "method": "subscribe",
                "params": {
                    token:token,
                    channel: "public:items:730:rub" // Или другой канал
                }
            }));
        });
```
Здесь все менялось в params
