using CitizenFX.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FiveLife.Server.Connection
{

    internal class QueueItem
    {

        internal int Position { get; set; }
        internal int Priority { get; set; }
        internal Player Player { get; private set; }

        public QueueItem(Player player, int priority)
        {
            Player = player;
            Priority = priority;
        }
    }

    internal class Queue
    {
        private List<QueueItem> items = new List<QueueItem>();

        public void Add(QueueItem item)
        {
            items.Add(item);
            Update();
        }

        public QueueItem Get(string steamId)
        {
            return items.FirstOrDefault(e => e.Player.Identifiers.FirstOrDefault() == steamId);
        }

        public QueueItem Get(Player player)
        {
            return Get(player.Identifiers.FirstOrDefault());
        }

        public void Add(Player player, int priority)
        {
            var pos = -1;
            foreach (var item in items)
            {
                // we have a priority
                if (priority > 0)
                {
                    // you don't, we cut in front
                    if (item.Priority == 0)
                    {
                        pos = items.IndexOf(item);
                        break;
                    }
                    else if (priority > item.Priority)
                    {
                        pos = items.IndexOf(item);
                        break;
                    }

                }
            }

            if (pos != -1)
            {
                items.Insert(pos, new QueueItem(player, priority));
            }
            else
            {
                items.Add(new QueueItem(player, priority));
            }

            Update();
        }

        public void Contains(QueueItem item)
        {
            items.Contains(item);
        }

        public bool Contains(Player player)
        {
            return Contains(player.Identifiers.FirstOrDefault());
        }

        public bool Contains(string steamId)
        {
            return items.FirstOrDefault(e => e.Player.Identifiers.FirstOrDefault() == steamId) != null;
        }

        public void Remove(QueueItem item)
        {
            items.Remove(item);
        }

        public void Remove(Player player)
        {
            Remove(player.Identifiers.FirstOrDefault());
        }

        public void Remove(string steamId)
        {
            var item = items.FirstOrDefault(e => e.Player.Identifiers.FirstOrDefault() == steamId);
            if (item == null) return;

            Remove(item);
            Update();
        }

        public int Count()
        {
            return items.Count;
        }

        private void Update()
        {
            foreach (var item in items)
            {
                var index = items.IndexOf(item);
                item.Position = index + 1;
            }
        }

    }
}