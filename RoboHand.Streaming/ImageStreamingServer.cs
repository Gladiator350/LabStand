using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace RoboHand.Streaming
{

    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class ImageStreamingServer:IDisposable
    {

        private List<Socket> _Clients;
        private Thread _Thread;
        private Socket Server;
        

        public ImageStreamingServer(IEnumerable<byte[]> imagesSource)
        {

            _Clients = new List<Socket>();
            _Thread = null;

            this.ImagesSource = imagesSource;
            this.Interval = 50;

        }


        /// <summary>
        /// Gets or sets the source of images that will be streamed to the 
        /// any connected client.
        /// </summary>
        public IEnumerable<byte[]> ImagesSource { get; set; }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<Socket> Clients { get { return _Clients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning => _Thread is { IsAlive: true };

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port = 8080)
        {

            lock (this)
            {
                _Thread = new Thread(new ParameterizedThreadStart(ServerThread));
                _Thread.IsBackground = true;
                _Thread.Start(port);
            }

        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                try
                {
                    Server.Close();
                    _Thread = null;
                    lock (_Clients)
                    {

                        foreach (var s in _Clients)
                        {
                            try
                            {
                                s.Close();
                            }
                            catch { }
                        }
                        _Clients.Clear();

                    }
                    _Thread.Join();
                }
                finally
                {

                    lock (_Clients)
                    {
                        
                        foreach (var s in _Clients)
                        {
                            try
                            {
                                s.Close();
                            }
                            catch { }
                        }
                        _Clients.Clear();

                    }

                    _Thread = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="state"></param>
        private void ServerThread(object state)
        {

            try
            {
                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Server.Bind(new IPEndPoint(IPAddress.Any,(int)state));
                Server.Listen(10);

                Console.WriteLine($"Server started on port {state}.");
                foreach (Socket client in Server.IncommingConnectoins())
                {
                    if (_Thread.ThreadState == ThreadState.AbortRequested) break;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
                }
            
            }
            catch { }
            finally
            {
                lock (_Clients)
                {

                    foreach (var s in _Clients)
                    {
                        try
                        {
                            s.Close();
                        }
                        catch { }
                    }
                    _Clients.Clear();

                }

                _Thread = null;
            }

            this.Stop();
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientThread(object client)
        {

            Socket socket = (Socket)client;
            Console.WriteLine($"New client from {socket.RemoteEndPoint.ToString()}");

            lock (_Clients)
                _Clients.Add(socket);

            try
            {
                using (MjpegWriter wr = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    // Writes the response header to the client.
                    wr.WriteHeader();

                    // Streams the images from the source to the client.
                    foreach (var img in ImagesSource.Streams())
                    {
                        if (this.Interval > 0)
                            Thread.Sleep(this.Interval);
                        
                        wr.Write(img);
                    }

                }
            }
            catch { }
            finally
            {
                lock (_Clients)
                {
                    _Clients.Remove(socket);
                }
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion
    }

    static class Extensions
    {

        public static IEnumerable<Socket> IncommingConnectoins(this Socket server)
        {
            while(true)
                yield return server.Accept();
        }
        
        internal static IEnumerable<MemoryStream> Streams(this IEnumerable<byte[]> source)
        {
            MemoryStream ms = new MemoryStream();

            foreach (var img in source)
            {
                ms.SetLength(0);
                ms.Write(img);
                yield return ms;
            }

            ms.Close();
            ms = null;

            yield break;
        }

    }
    
}
