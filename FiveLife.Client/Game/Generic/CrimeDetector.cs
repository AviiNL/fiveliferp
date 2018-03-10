using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Generic
{
    public class CrimeDetector : FiveLifeScript
    {
        public override void Initialize()
        {
            API.DisablePoliceReports();
        }

        public override async Task Loop()
        {
            if (CitizenFX.Core.Game.Player.WantedLevel > 0)
            {
                CitizenFX.Core.Game.Player.WantedLevel = 0;
            }
        }

    }
}
