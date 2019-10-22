using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.Animation
{
    public class Point : FiveLifeScript
    {
        private bool mp_pointing = false;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override async Task Loop()
        {
            var ped = CitizenFX.Core.Game.Player.Character;

            if (CitizenFX.Core.Game.IsControlPressed(0, CitizenFX.Core.Control.SpecialAbilitySecondary))
            {
                if (mp_pointing == false)
                {
                    StartPointing();
                }
                mp_pointing = true;
            }
            else
            {
                if (mp_pointing == true)
                {
                    StopPointing();
                }
                mp_pointing = false;
            }

            if (mp_pointing)
            {

                float camPitch = Function.Call<float>(Hash.GET_GAMEPLAY_CAM_RELATIVE_PITCH);
                float camHeading = Function.Call<float>(Hash.GET_GAMEPLAY_CAM_RELATIVE_HEADING);

                if (camPitch < -70f)
                {
                    camPitch = -70f;
                }
                else if (camPitch > 42f)
                {
                    camPitch = 42f;
                }

                camPitch = (camPitch + 70f) / 112f;

                float cosCamHeading = (float)Math.Cos(camHeading);
                float sinCamHeading = (float)Math.Sin(camHeading);

                if (camHeading < -180f)
                {
                    camHeading = -180f;
                }
                else if (camHeading > 180f)
                {
                    camHeading = 180f;
                }

                camHeading = (camHeading + 180f) / 360f;

                Function.Call((Hash)0xD5BB4025AE449A4E, ped, "Pitch", camPitch);
                Function.Call((Hash)0xD5BB4025AE449A4E, ped, "Heading", camHeading * -1.0 + 1.0);
                Function.Call((Hash)0xB0A6CFD2C69C1088, ped, "isBlocked", false);
                Function.Call((Hash)0xB0A6CFD2C69C1088, ped, "isFirstPerson", Function.Call<int>((Hash)0xEE778F8C7E1142E2, Function.Call<int>((Hash)0x19CAFA3C87F7C2FF)) == 4);
            }
            else
            {
                StopPointing();
            }
        }


        private async void StartPointing()
        {
            var ped = CitizenFX.Core.Game.Player.Character;
            API.RequestAnimDict("anim@mp_point");
            while (!API.HasAnimDictLoaded("anim@mp_point"))
            {
                await Delay(0);
            }

            API.SetPedCurrentWeaponVisible(ped.GetHashCode(), false, true, true, true);
            API.SetPedConfigFlag(ped.GetHashCode(), 36, true);
            Function.Call(Hash._TASK_MOVE_NETWORK, ped, "task_mp_pointing", 0.5f, 0, "anim@mp_point", 24);
            API.RemoveAnimDict("anim@mp_point");
        }

        private async void StopPointing()
        {
            var ped = CitizenFX.Core.Game.Player.Character;
            Function.Call((Hash)0xD01015C7316AE176, ped, "stop");
            if (!ped.IsInjured)
            {
                ped.Task.ClearSecondary();
            }

            API.SetPedConfigFlag(ped.GetHashCode(), 36, false);
        }

    }
}
