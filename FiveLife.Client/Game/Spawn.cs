using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using FiveLife.Shared.Entity;

namespace FiveLife.Client.Game
{
    public class Spawn : FiveLifeScript
    {
        bool isSpawning = false;
        bool gameEntered = false;
        bool isGameUIOpen = false;

        public override void Initialize()
        {
            RegisterEvent<Character>("fivelife.character.selected", OnGameEnter);
        }

        private async void OnGameEnter(Character obj)
        {
            isSpawning = true;

            Screen.Fading.FadeOut(100);
            while (Screen.Fading.IsFadingOut)
                await Delay(0);

            CitizenFX.Core.Game.Player.Character.Show();
            CitizenFX.Core.Game.Player.Character.FadeOut();
            
            Data.Character = obj;

            CitizenFX.Core.Game.Player.CanControlCharacter = false;
            await CitizenFX.Core.Game.Player.Spawn(obj);
            CitizenFX.Core.Game.Player.CanControlCharacter = true;

            // TODO: Move to UI handler
            Screen.Hud.IsVisible = true;
            Screen.Hud.IsRadarVisible = true;

            Screen.Fading.FadeIn(500);
            while (Screen.Fading.IsFadingIn)
                await Delay(0);
            
            Screen.Effects.Stop();

            isSpawning = false;
            gameEntered = true;

            TriggerEvent("fivelife.character.spawned");
            CitizenFX.Core.Game.Player.Character.FadeIn();
        }

        public override async Task Loop()
        {
            if (isSpawning)
            {
                CitizenFX.Core.Game.DisableAllControlsThisFrame(0);
                CitizenFX.Core.Game.DisableAllControlsThisFrame(1);
                CitizenFX.Core.Game.DisableAllControlsThisFrame(2);
            }


             // This should totally not be in Spawn....
            // Ingame UI
            if (gameEntered)
            {
                if (API.IsPauseMenuActive() && isGameUIOpen)
                {
                    HideUI();
                }
                else if (!API.IsPauseMenuActive() && !isGameUIOpen)
                {
                    ShowUI();
                }
            }
        }

        private void ShowUI()
        {
            isGameUIOpen = true;
            NUI.Show(NUI.Page.Game);
        }

        private void HideUI()
        {
            isGameUIOpen = false;
            NUI.Close();
        }
    }
}
