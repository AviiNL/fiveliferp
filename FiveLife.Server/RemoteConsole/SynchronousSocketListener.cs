using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.RemoteConsole
{
    public class SynchronousSocketListener
    {
        public delegate void MessageReceived(string message, ClientHandler client);
        public event MessageReceived OnMessageReceived;


        public delegate void ClientConnected(ClientHandler client);
        public event ClientConnected OnClientConnected;

        private const int portNum = 10116;

        public void StartListening()
        {
            Console.WriteLine("Remote Console ready.");
            ClientService ClientTask;

            // Client Connections Pool
            ClientConnectionPool ConnectionPool = new ClientConnectionPool();

            // Client Task to handle client requests
            ClientTask = new ClientService(ConnectionPool);

            ClientTask.Start();

            TcpListener listener = new TcpListener(IPAddress.Any, portNum);
            try
            {
                listener.Start();

                int ClientNbr = 0;

                // Start listening for connections.
                while (true)
                {

                    TcpClient handler = listener.AcceptTcpClient();

                    if (handler != null)
                    {
                        Console.WriteLine("Client#{0} accepted!", ++ClientNbr);

                        // An incoming connection needs to be processed.
                        var clientHandler = new ClientHandler(ClientNbr, handler);
                        OnClientConnected?.Invoke(clientHandler);

                        clientHandler.OnMessageReceived += ClientHandler_OnMessageReceived;
                        ConnectionPool.Enqueue(clientHandler);
                    }
                    else
                        break;
                }
                listener.Stop();

                // Stop client requests handling
                ClientTask.Stop();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ClientHandler_OnMessageReceived(string message, ClientHandler client)
        {
            OnMessageReceived?.Invoke(message, client);
        }
    }
}
