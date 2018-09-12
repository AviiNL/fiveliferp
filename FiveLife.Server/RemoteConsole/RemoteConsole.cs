using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FiveLife.Server.RemoteConsole
{
    public class RemoteConsole : FiveLifeScript
    {
        SynchronousSocketListener listener;

        public async override void Initialize()
        {
            ThreadPool.QueueUserWorkItem(e =>
            {
                listener = new SynchronousSocketListener();
                listener.OnMessageReceived += Listener_OnMessageReceived;
                listener.StartListening();
            });
        }

        private void Listener_OnMessageReceived(string message, ClientHandler client)
        {
            Debug.WriteLine($"{client.Id}: {message}");
            client.Send(message);// echo back (for now)
        }
    }
}
