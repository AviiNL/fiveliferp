using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Client.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game
{
    public class Data : BaseScript
    {
        
        public static Shared.Entity.Player Player;
        public static Shared.Entity.Character Character;
        public static IEnumerable<Shared.Entity.Room> Rooms;

        public Data()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Converters = new List<JsonConverter> { new Database.Converter.MicrosecondEpochConverter() }
            };

            Tick += Data_Tick;
        }

        private bool firstTick = true;
        public async Task Data_Tick()
        {
            if (firstTick)
            {
                Repository<Shared.Entity.Player>.Init(this.EventHandlers);
                Function.Call(Hash._USE_FREEMODE_MAP_BEHAVIOR, true);
                Function.Call(Hash.ADD_TEXT_ENTRY, "FE_THDR_GTAO", "FiveLife RolePlay");
                Function.Call(Hash.NETWORK_SET_FRIENDLY_FIRE_OPTION, true);
                Function.Call(Hash.SET_CAN_ATTACK_FRIENDLY, CitizenFX.Core.Game.Player.Character, true, true);

                // Start data initialization here
                Player = await Repository<Shared.Entity.Player>.GetBySource();
                Rooms = await Repository<Shared.Entity.Room>.GetAll();

                TriggerEvent("fivelife.game.initialized");
                
                Tick -= Data_Tick;
                firstTick = false;
                return;
            }
        }
        

    }
}
