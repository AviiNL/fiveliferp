using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using FiveLife.Client.Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Generic
{
    public class Death : FiveLifeScript
    {
        public const int RespawnTimeoutInSeconds = 10;
        public const int CostNoEMT = 50000;
        public const int CostEMT = 250000;

        public DateTime? DiedAt { get; set; } = null;

        private bool canRespawn = false;
        private bool isDead = false;
        public override void Initialize()
        {
            API.RegisterCommand("respawn", new Action(Respawn), false);
            RegisterEvent<Vector4>("fivelife.generic.death.respawn", OnRespawn);
        }

        private async void OnRespawn(Vector4 location)
        {
            isDead = false;
            await CitizenFX.Core.Game.Player.Spawn(new Vector3(location.X, location.Y, location.Z), location.W, false);

            Screen.Fading.FadeIn(5000);
            while (Screen.Fading.IsFadingIn)
                await Delay(0);
        }

        private void Respawn()
        {
            if (!canRespawn) return;

            FireEvent("fivelife.generic.death.respawn", new Vector4(CitizenFX.Core.Game.Player.Character.Position, 0)); // TODO: Get hospital location
        }

        public override async Task Loop()
        {
            if (CitizenFX.Core.Game.Player.Character.IsDead)
                isDead = true;

            if (isDead)
            {
                API.ResetPedRagdollTimer(API.GetPlayerPed(-1));
                if (DiedAt == null)
                {
                    await Delay(2000);
                    Function.Call(Hash.RESURRECT_PED, CitizenFX.Core.Game.Player.Character);
                    Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, CitizenFX.Core.Game.Player.Character);
                    CitizenFX.Core.Game.Player.Character.IsInvincible = true;
                    CitizenFX.Core.Game.Player.Character.Health = 0;
                    await Delay(100);
                    CitizenFX.Core.Game.Player.Character.Ragdoll();

                    DiedAt = DateTime.UtcNow;
                }

                //Screen.Fading.FadeOut(750);
                //while (Screen.Fading.IsFadingOut)
                //    await Delay(0);

                //CitizenFX.Core.Game.Player.Character.IsInvincible = true;
                //await Delay(20);


                // await Delay(700);
                //Screen.Fading.FadeIn(1000);
                //while (Screen.Fading.IsFadingIn)
                //    await Delay(0);


            }
            else
            {
                DiedAt = null;
                canRespawn = false;
            }

            if (!DiedAt.HasValue) return;

            var diff = Math.Round((DiedAt.Value - DateTime.UtcNow).TotalSeconds + RespawnTimeoutInSeconds);

            if (diff > 0)
            {
                Subtitle.Draw(String.Format("You are incapacitated, please wait ~r~{0}~w~ seconds to respawn or wait for an EMT", diff));
                canRespawn = false;
            }
            else
            {
                Subtitle.Draw(String.Format("You are incapacitated, you can ~y~/respawn~w~ for ~g~$ {0:#.00}~w~ or wait for an EMT", Convert.ToDecimal(CostNoEMT) / 100));
                canRespawn = true;
            }

        }

    }
}
