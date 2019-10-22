using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Animation
{
    public class Handsup : FiveLifeScript
    {
        private bool isHandsup = false;

        private bool firstTick = true;

        public override async Task Loop()
        {
            if (firstTick && !API.HasAnimDictLoaded("ped"))
            {
                API.RequestAnimDict("ped");
                while (!API.HasAnimDictLoaded("ped"))
                    await Delay(0);

                firstTick = false;
                return;
            }

            if (!CitizenFX.Core.Game.IsControlPressed(0, CitizenFX.Core.Control.CharacterWheel) && CitizenFX.Core.Game.IsControlJustPressed(0, CitizenFX.Core.Control.ReplayTimelinePickupClip))
            {
                if (CitizenFX.Core.Game.Player.Character.IsInVehicle())
                {
                    if (!isHandsup) return;
                }

                if (!isHandsup)
                {
                    API.TaskPlayAnim(CitizenFX.Core.Game.Player.Character.GetHashCode(), "ped", "handsup_enter", 8.0f, 4.0f, -1, 50, 0f, false, false, false);
                }
                else
                {
                    CitizenFX.Core.Game.Player.Character.Task.ClearAll();
                }

                isHandsup = !isHandsup;
            }
        }
    }
}
