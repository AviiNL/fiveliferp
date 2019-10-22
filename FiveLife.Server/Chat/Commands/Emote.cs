using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using FiveLife.Shared.Entity;

namespace FiveLife.Server.Chat.Commands
{
    public class Emote : ChatCommand
    {
        public override void Handle(CitizenFX.Core.Player source, ChatMessage data)
        {
            // FiveLifeScript.TriggerClientEvent("fivelife.player.emote", "");
        }
    }
}
