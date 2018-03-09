using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Server.Chat.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Chat
{
    public class Chat : FiveLifeScript
    {

        private Dictionary<string, Action<Player, Shared.Entity.ChatMessage>> Actions = new Dictionary<string, Action<Player, Shared.Entity.ChatMessage>>();

        public override void Initialize()
        {
            RegisterEvent<Player, Shared.Entity.Character, string, Vector3>("fivelife.chat.message", OnMessageSend);

            EventHandlers.Add("__cfx_internal:commandFallback", new Action<Player, string>(CommandFallback));

            // Load all chat commands
            var type = typeof(ChatCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOf(typeof(ChatCommand)));

            foreach (var command in types)
            {
                Actions.Add(command.Name.ToLower().TrimStart('_'), new Action<Player, Shared.Entity.ChatMessage>(((ChatCommand)Activator.CreateInstance(command)).Handle));
            }

        }

        private async void CommandFallback([FromSource] Player player, string raw)
        {
            player.TriggerEvent("fivelife.chat.commandFallback", JsonConvert.SerializeObject(new Shared.Entity.ChatMessage()
            {
                Character = null,
                Message = raw,
                Position = Vector3.Zero
            }));

            Function.Call(Hash.CANCEL_EVENT);
        }

        private void OnMessageSend([FromSource] Player player, Shared.Entity.Character arg1, string arg2, Vector3 arg3)
        {
            if (arg2.Length == 0) return;
            var message = new Shared.Entity.ChatMessage()
            {
                Character = arg1,
                Message = arg2,
                Position = arg3
            };

            if (arg2.Substring(0, 1) == "/")
            {
                ExecuteCommand(player, message); // each command will deal with sending it back to the client
                return;
            }

            message.Message = $"{arg1.FirstName} {arg1.LastName}: {arg2}";
            Broadcast(message);
        }

        private void ExecuteCommand(Player player, Shared.Entity.ChatMessage command)
        {
            var parts = command.Message.Substring(1).Split(' ').ToList();
            var cmd = parts[0].ToLower();
            if (!Actions.ContainsKey(cmd))
            {
                command.Message = $"[color=red]Unknown command: {cmd}[/color]";
                Send(player, command);
                return;
            }
            Actions[cmd](player, command);
        }

        private void Broadcast(Shared.Entity.ChatMessage message)
        {
            foreach (var p in new PlayerList())
            {
                Send(p, message);
            }
        }

        private void Send(Player player, Shared.Entity.ChatMessage data)
        {
            player.TriggerEvent("fivelife.chat.addMessage", JsonConvert.SerializeObject(data));
        }


    }
}
