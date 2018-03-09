using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using FiveLife.Shared.Entity;

namespace FiveLife.Server.Chat.Commands
{
    public class OOC : ChatCommand
    {

        public override async void Handle(CitizenFX.Core.Player source, ChatMessage data)
        {
            var parts = data.Message.Split(' ').ToList();
            parts.RemoveAt(0);
            var message = String.Join(" ", parts);

            data.Message = $"[color=gray]{data.Character.FirstName} {data.Character.LastName} [Local OOC]: {message}[/color]";
            Broadcast(data);
        }

    }
}
