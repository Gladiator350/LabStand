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
            listener.Prefixes.Add("http://+:8080/");
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
                        IPEndPoint remoteEndPoint = context.Request.RemoteEndPoint;  // Это объект IPEndPoint
                        string ipAddressString = remoteEndPoint.Address.ToString(); // Получаем строку IP-адреса
                        Console.WriteLine($"Client IP: {ipAddressString}");         // Выводим IP-адрес в консоль
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
                        
                        var commandAngles = message.Split("cor");
                        if (message.StartsWith("cor"))
                        {
                            
                            if (!int.TryParse(commandAngles[0], out int Uid))
                            {
                                Console.WriteLine("Отсутствует вариант");
                                continue;
                            }
                            var angles = new List<Angles>();
                            foreach (var commandAngle in commandAngles)
                            {
                                angles.Add(Angles.FromCommandText(commandAngle));
                            }
                            var command = new Command
                            {
                                Angles = angles.ToArray(),
                                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Uid = Uid
                            };
                            //textToSend += AnglesCalculator.Check(angles, _context.Variants.First(x => x.Id == angles.Id)).ToString();
                            Console.WriteLine(textToSend);
                            await _context.Commands.AddAsync(command);
                            await _context.SaveChangesAsync();
                            actionOnReceived(command);
                        }
                        byte[] textBytes = Encoding.UTF8.GetBytes(textToSend);
                        Console.WriteLine(textBytes);
                        byte[] combinedBuffer = new byte[buffer.Length + textBytes.Length];
                        Buffer.BlockCopy(buffer, 0, combinedBuffer, 0, buffer.Length);
                        Buffer.BlockCopy(textBytes, 0, combinedBuffer, buffer.Length, textBytes.Length);
                        
                        // Эхо-ответ клиенту
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(combinedBuffer, 0, combinedBuffer.Length),
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: CancellationToken.None);
                            
                        Console.WriteLine("Sent: " + message);
                        var i = +1;
                        //Console.WriteLine(Clients);
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