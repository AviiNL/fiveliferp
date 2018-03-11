using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game
{
    class Tweaks : FiveLifeScript
    {
        public override void Initialize()
        {

        }

        public override async Task Loop()
        {
            Function.Call(Hash.HIDE_LOADING_ON_FADE_THIS_FRAME);

            // Disable melee with R key
            CitizenFX.Core.Game.DisableControlThisFrame(0, CitizenFX.Core.Control.MeleeAttackLight);
            
            // Remove nametag
            foreach (var p in (new CitizenFX.Core.PlayerList()))
            {
                Function.Call(Hash.REMOVE_MP_GAMER_TAG, Function.Call<int>((Hash)0xBFEFE3321A3F5015, p.Character, "", false, false, "", 0));
            }

            // Disable auto-healing
            Function.Call(Hash.SET_PLAYER_HEALTH_RECHARGE_MULTIPLIER, CitizenFX.Core.Game.Player, 0f);
        }

    }
}
