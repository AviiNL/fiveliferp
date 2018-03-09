using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Chat
{
    public class Chat : FiveLifeScript
    {
        public Chat()
        {

        }

        public override async void Initialize()
        {
            API.SetTextChatEnabled(false);

            RegisterEvent<Shared.Entity.ChatMessage>("fivelife.chat.commandFallback", CommandFallback);
            RegisterEvent<Shared.Entity.ChatMessage>("fivelife.chat.addMessage", OnAddMessage);

            RegisterNUICallback<Message>("chat.result", OnChatResult);

            Tick += SChat_Tick;
        }

        private async void CommandFallback(Shared.Entity.ChatMessage raw)
        {
            FireServerEvent("fivelife.chat.message", CitizenFX.Core.Game.Player.ServerId, Game.Data.Character, "/" + raw.Message, CitizenFX.Core.Game.Player.Character.Position);
        }

        private async void OnAddMessage(Shared.Entity.ChatMessage message)
        {
            if (message.Position == Vector3.Zero || message.Position.DistanceToSquared(CitizenFX.Core.Game.Player.Character.Position) < 1200)
            {
                API.SendNuiMessage(JsonConvert.SerializeObject(new
                {
                    component = "chat",
                    type = "ON_MESSAGE",
                    message = message.Message
                }));
            }
        }

        private async void OnChatResult(Message data)
        {
            API.SetNuiFocus(false, false);
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                component = "chat",
                type = "ON_CLOSE",
            }));

            if (data == null || data.message == null) return;
            if (data.message.Length == 0) return;

            if (data.message.Substring(0, 1) == "/")
            {
                API.ExecuteCommand(data.message.Substring(1));
                return;
            }

            FireServerEvent("fivelife.chat.message", CitizenFX.Core.Game.Player.ServerId, Game.Data.Character, data.message, CitizenFX.Core.Game.Player.Character.Position);

        }

        private async Task SChat_Tick()
        {
            if (Game.Data.Character == null) return;
            if (!CitizenFX.Core.Game.IsControlPressed(0, (Control)132) && CitizenFX.Core.Game.IsControlJustReleased(0, Control.MpTextChatAll))
            {
                API.SendNuiMessage(JsonConvert.SerializeObject(new
                {
                    component = "chat",
                    type = "ON_OPEN"
                }));
                API.SetNuiFocus(true, false);
            }
        }
    }
}
