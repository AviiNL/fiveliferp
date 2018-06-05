using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Sync
{
    public class Room : FiveLifeScript
    {
        public override void Initialize()
        {
            RegisterEvent<Player, Shared.Entity.Room, Shared.Entity.Character>("fivelife.room.enter", OnRoomEnter);
            RegisterEvent<Player, Shared.Entity.Room>("fivelife.room.leave", OnRoomLeft);
        }

        private void OnRoomEnter([FromSource] Player source, Shared.Entity.Room obj, Shared.Entity.Character character)
        {
            var room = Database.SqLite.Repository<Shared.Entity.Room>.GetById(obj.Id);
            if (room == null) return;

            if (!(room.Owner.Id == character.Id || room.Allowed.FirstOrDefault(x => x.Id == character.Id) != null))
            {
                FireEvent(room.CurrentlyInside.ToArray(), "fivelife.room.enter", false, room);
                return;
            }

            room.CurrentlyInside.Add(Int32.Parse(source.Handle));
            Database.SqLite.Repository<Shared.Entity.Room>.Update(room);

            FireEvent(source, "fivelife.room.enter", true, room);
            FireEvent("fivelife.room.joined", Int32.Parse(source.Handle), room);
        }

        private void OnRoomLeft([FromSource] Player source, Shared.Entity.Room obj)
        {
            var room = Database.SqLite.Repository<Shared.Entity.Room>.GetById(obj.Id);

            room.CurrentlyInside.RemoveAll(e => e == Int32.Parse(source.Handle));
            Database.SqLite.Repository<Shared.Entity.Room>.Update(room);

            FireEvent(source, "fivelife.room.exit", room);
            FireEvent("fivelife.room.left", Int32.Parse(source.Handle));
        }
    }
}
