using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using FiveLife.Shared.Entity;
using Newtonsoft.Json;

namespace FiveLife.Client.Login
{
    class SelectCharacter : FiveLifeScript
    {
        private CharacterCreator.CharacterCreator characterCreator = new CharacterCreator.CharacterCreator();

        public override void Initialize()
        {
            API.ShutdownLoadingScreenNui();

            RegisterNUICallback<Shared.Entity.Character>("fivelife.nui.character.selected", CharacterSelected);
            RegisterNUICallback("fivelife.nui.character.create", CreateNewCharacter);
            RegisterNUICallback<Shared.Entity.Character>("fivelife.nui.character.finished", CharacterFinished);
            RegisterNUICallback("fivelife.nui.character.cancel", CreateNewCancel);

            NUI.Open(NUI.Page.CharacterSelect, Game.Data.Player);
        }
        
        private async void CreateNewCharacter()
        {
            Screen.Fading.FadeOut(500);

            while (Screen.Fading.IsFadingOut)
                await Delay(10);

            NUI.Close();

            await characterCreator.Start();

            Screen.Fading.FadeIn(2500);

            while (Screen.Fading.IsFadingIn)
                await BaseScript.Delay(0);
            
        }

        private async void CreateNewCancel()
        {
            
        }

        private async void CharacterFinished(Character obj)
        {



            // when done, do this to get in game
            //CharacterSelected(obj);
        }

        private async void CharacterSelected(Character obj)
        {
            Debug.WriteLine("{0}", $"Character selected! {obj.FirstName} {obj.LastName}");

            Screen.Fading.FadeOut(10);

            while (Screen.Fading.IsFadingOut)
                await Delay(10);

            NUI.Close();

            FireEvent("fivelife.character.selected", obj);
            FireServerEvent("fivelife.character.selected", obj);
        }

        public override async Task Loop()
        {
            characterCreator.Update();
        }
    }
}
