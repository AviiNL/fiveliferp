using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Generic
{
    public class DisableWeaponDrop : FiveLifeScript
    {
        public override async Task Loop()
        {
            foreach(var ped in new PedsPool())
            {
                ped.DropsWeaponsOnDeath = false;
            }

            await Delay(1000);
        }
    }
}
