using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiveLife.Server.Remote.Connection
{
    public class ConsoleClient
    {
        public delegate void Connect();
        public event Connect OnConnect;

        public delegate void Disconnect();
        public event Disconnect OnDisconnect;

        public delegate void DataReceived(string data);
        public event DataReceived OnDataReceived;

        private const int portNum = 10116;
        TcpClient tcpClient;
        NetworkStream networkStream;
        public bool isConnected = false;

        public void Start()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse("127.0.0.1"), portNum);
                networkStream = tcpClient.GetStream();
                isConnected = true;

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    while (isConnected)
                    {
                        try
                        {
                            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                            int BytesRead = networkStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);
                            string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                            returndata.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList().ToList().ForEach(s => OnDataReceived?.Invoke(s));
                        }
                        catch (IOException)
                        {
                            isConnected = false;
                            OnDisconnect?.Invoke();
                            Console.WriteLine("Disconnected");
                        }
                    }
                });
                OnConnect?.Invoke();
                Console.WriteLine("Connected.");
            }
            catch (SocketException)
            {
                isConnected = false;
                OnDisconnect?.Invoke();
                Console.WriteLine("Unable to connect, is the server running?");
            }
        }

        public void Close()
        {
            isConnected = false;
            networkStream.Close();
            tcpClient.Close();
        }

        public void Send(string data)
        {
            if (data[data.Length - 1] != '\n')
                data += '\n';

            try
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(data);
                networkStream.Write(sendBytes, 0, sendBytes.Length);
            }
            catch (SocketException)
            {
                Console.WriteLine("Not Connected");
            }
        }

    }
}
