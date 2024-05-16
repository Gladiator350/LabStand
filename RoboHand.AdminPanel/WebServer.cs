using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Windows.Documents;
using WpfApp23.Models;
using WpfApp23.ViewModels;

namespace WpfApp23

{
    class WebServer
    {
        public static int clientCount = 0;
        public static List<String> Clients = new List<String>();
        
        public static async Task Start(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/ws/");
            listener.Start();
            Console.WriteLine("Listening...");

            var webSockets = new WebSocket[10]; // Ограничение на 10 клиентов

            try
            {
                while (clientCount < 10)
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
                        webSockets[clientCount++] = webSocket;
                        Console.WriteLine(webSockets[clientCount]);

                        // Запускаем обработку сообщений для нового клиента в отдельном потоке
                        _ = HandleWebSocketAsync(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        clientCount -= 1;
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
        
        static async Task HandleWebSocketAsync(WebSocket webSocket)
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
                        if (message.Contains("cor"))
                        {
                            Angles angles = new Angles();
                            string[] answer = message.Split("");
                            var anglesAlpha = angles.Alpha;
                            int intValue = 0;
                            bool successV = Int32.TryParse(answer[0], out intValue);
                            if (successV)
                            {
                                angles.Id = intValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись Варианта");
                            }

                            float floatValue = 0;
                            bool successA = float.TryParse(answer[2], out floatValue);
                            if (successA)
                            {
                                angles.Alpha = floatValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись угла Альфа");
                            }

                            successA = float.TryParse(answer[3], out floatValue);
                            if (successA)
                            {
                                angles.Beta = floatValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись угла Бетта");
                            }

                            successA = float.TryParse(answer[4], out floatValue);
                            if (successA)
                            {
                                angles.Gamma = floatValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись угла Гамма");
                            }

                            successA = float.TryParse(answer[5], out floatValue);
                            if (successA)
                            {
                                angles.Theta = floatValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись угла Тетта");
                            }

                            successA = float.TryParse(answer[6], out floatValue);
                            if (successA)
                            {
                                angles.Omega = floatValue;
                            }
                            else
                            {
                                Console.WriteLine("некоректная запись угла Омега");
                            }
                            
                        }

                        Console.WriteLine(message);
                        

                        // Эхо-ответ клиенту
                        await webSocket.SendAsync(
                            new ArraySegment<byte>(buffer, 0, result.Count),
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
                            clientCount -= 1;
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