using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Chat.Commands
{
    abstract public class ChatCommand
    {
        public abstract void Handle(Player source, Shared.Entity.ChatMessage data);

        protected void Broadcast(Shared.Entity.ChatMessage data)
        {
            foreach (var p in new PlayerList())
            {
                Send(p, data);
            }
        }

        protected void Send(Player player, Shared.Entity.ChatMessage data)
        {
            player.TriggerEvent("fivelife.chat.addMessage", JsonConvert.SerializeObject(data));
        }
    }
}
