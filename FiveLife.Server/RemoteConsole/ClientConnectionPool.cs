﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.RemoteConsole
{
    internal class ClientConnectionPool
    {
        // Creates a synchronized wrapper around the Queue.
        private Queue SyncdQ = Queue.Synchronized(new Queue());

        public void Enqueue(ClientHandler client)
        {
            SyncdQ.Enqueue(client);
        }

        public ClientHandler Dequeue()
        {
            return (ClientHandler)(SyncdQ.Dequeue());
        }

        public int Count {
            get { return SyncdQ.Count; }
        }

        public object SyncRoot {
            get { return SyncdQ.SyncRoot; }
        }

    } // class ClientConnectionPool
}
