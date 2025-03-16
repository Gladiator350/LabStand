using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using WpfApp23.ApplicationContexts;
using WpfApp23.Calculate;
using WpfApp23.Models;

namespace WpfApp23

{
    public class WebServer(ApplicationContext context)
    {
        private readonly ApplicationContext _context = context;
        private int _clientCount = 0;

        public async Task Start(Action<Command> actionOnReceived)
        {

            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:9011/ws/");
            listener.Start();
            Console.WriteLine("Listening...");

            var webSockets = new WebSocket[10]; // Ограничение на 10 клиентов

            try
            {
                while (_clientCount < 10)
                {
                    var context = await listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        IPEndPoint remoteEndPoint = context.Request.RemoteEndPoint; // Это объект IPEndPoint
                        string ipAddressString = remoteEndPoint.Address.ToString(); // Получаем строку IP-адреса
                        Console.WriteLine($"Client IP: {ipAddressString}"); // Выводим IP-адрес в консоль
                        //Clients.Add(context.Request.RemoteEndPoint.ToString());
                        var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        var webSocket = webSocketContext.WebSocket;
                        string clientIp = context.Request.RemoteEndPoint.ToString();
                        Console.WriteLine(clientIp);
                        // Сохраняем WebSocket клиента
                        webSockets[_clientCount++] = webSocket;
                        Console.WriteLine(webSockets[_clientCount]);

                        // Запускаем обработку сообщений для нового клиента в отдельном потоке
                        _ = HandleWebSocketAsync(webSocket, actionOnReceived);
                    }
                    else
                    {
                        Console.WriteLine("не получилось");
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
            finally
            {
                listener.Close();
            }
        }

        async Task HandleWebSocketAsync(WebSocket webSocket, Action<Command> actionOnReceived)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var textToSend = "";

                        // Разделяем строку по "cor"
                        var commandParts = message.Split("cor", StringSplitOptions.RemoveEmptyEntries);

                        if (commandParts.Length > 0)
                        {
                            // Первая часть — это Uid
                            if (!int.TryParse(commandParts[0].Trim(), CultureInfo.InvariantCulture, out int Uid))
                            {
                                Console.WriteLine("Отсутствует или некорректен Uid");
                                continue;
                            }

                            // Остальные части — это данные для Angles
                            var angles = new List<Angles>();
                            for (int i = 1; i < commandParts.Length; i++)
                            {
                                var anglesData = commandParts[i].Trim();
                                angles.Add(Angles.FromCommandText(anglesData));
                            }

                            // Создаем команду
                            var command = new Command
                            {
                                Angles = angles,
                                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Uid = Uid
                            };

                            // Обрабатываем углы
                            foreach (var angle in angles)
                            {
                                var variant = _context.Variants.FirstOrDefault(x => x.Id == command.Uid);
                                if (variant != null)
                                {
                                    textToSend += AnglesCalculator.Check(angle, variant).ToString();
                                }
                            }

                            // Сохраняем команду в базу данных
                            await _context.Commands.AddAsync(command);
                            await _context.SaveChangesAsync();

                            // Вызываем действие для обработки команды
                            actionOnReceived(command);

                            // Отправляем ответ клиенту
                            byte[] textBytes = Encoding.UTF8.GetBytes(textToSend);
                            await webSocket.SendAsync(
                                new ArraySegment<byte>(textBytes),
                                WebSocketMessageType.Text,
                                endOfMessage: true,
                                cancellationToken: CancellationToken.None);

                            Console.WriteLine("Sent: " + textToSend);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Закрыть соединение, если клиент отправил сообщение о закрытии
                        if (result.CloseStatus != null)
                        {
                            _clientCount -= 1;
                            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                                CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("WebSocket Exception: " + e);
            }
            finally
            {
                webSocket.Dispose();
            }
        }
    }
}