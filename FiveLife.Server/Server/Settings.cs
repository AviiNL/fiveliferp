using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Server
{
    public class Settings : BaseScript
    {
        private bool firstTick = true;

        public Settings()
        {
            Tick += Settings_Tick;
        }

        private async Task Settings_Tick()
        {
            if (firstTick)
            {
                Function.Call(Hash.SET_GAME_TYPE, "FiveLife");
                Function.Call(Hash.SET_MAP_NAME, "San Andreas");

                firstTick = false;
                Tick -= Settings_Tick;
            }
        }
    }
}
