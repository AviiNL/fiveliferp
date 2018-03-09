using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using FiveLife.Shared.Entity;

namespace FiveLife.Server.Chat.Commands
{
    public class AllowAccess : ChatCommand
    {
        public override async void Handle(CitizenFX.Core.Player source, ChatMessage data)
        {
            var rooms = Database.Repository<Room>.Find(e => e.Owner.Id == data.Character.Id);
            var ids = data.Message.Split(' ').Skip(1).Select(e => {
                if (int.TryParse(e, out int a))
                    return a;

                return -1;
            }).Where(a => a != -1);

            data.Message = "";
            List<string> responseLines = new List<string>();

            foreach (var room in rooms)
            {
                var line = $"Access granted to [color=red]{room.Name}[/color] for:[br]";
                var Outside = new Vector3(room.OutsideX, room.OutsideY, room.OutsideZ);
                var Inside = new Vector3(room.InsideX, room.InsideY, room.InsideZ);

                if (data.Position.DistanceToSquared(Outside) < 5 || data.Position.DistanceToSquared(Inside) < 5)
                {
                    var dirty = false;
                    foreach (var id in ids)
                    {
                        var character = Database.Repository<Character>.GetById(id);
                        if (character != null)
                        {
                            room.Allowed.Add(character);

                            line += $"{character.FirstName} {character.LastName}[br]";

                            dirty = true;
                        }
                    }

                    if (dirty)
                    {
                        Database.Repository<Room>.Update(room);
                        responseLines.Add(line);
                    }
                }
            }
            data.Message = String.Join("[br]", responseLines);
            Send(source, data);
        }
    }
}
