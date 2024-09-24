const https = require('https');
const axios = require('axios');
const WebSocket = require('ws');

// Замените 'your_secret_key' на ваш секретный ключ
const apiKey = 'FmH4d6naEVI6mFxmEUDiN4EPiM72d2O';


async function getWSToken() {
    try {
        const response = await axios.get(`https://market.csgo.com/api/v2/get-ws-token?key=FmH4d6naEVI6mFxmEUDiN4EPiM72d2O`);
        const token = response.data.token;
        console.log(`Received token: ${token}`);
        return token;
      } catch (error) {
        console.error('Error fetching token:', error);
        throw error;
      }
}


async function connectToWebSocket() {
    try {
        const token = await getWSToken();
        const wsUrl = `wss://wsprice.csgo.com/connection/websocket`;
        const ws = new WebSocket(wsUrl);

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

        ws.on('message', (data) => {
            const message = JSON.parse(data);
            console.log('Received message:', message);
        });

        ws.on('close', () => {
            console.log('WebSocket connection closed');
        });

        ws.on('error', (error) => {
            console.error('WebSocket error:', error);
        });
    } catch (error) {
        console.error('Error connecting to WebSocket:', error);
    }
}

// Запускаем подключение
connectToWebSocket();
