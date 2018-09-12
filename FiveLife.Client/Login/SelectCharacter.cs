using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using FiveLife.NativeUI;
using FiveLife.Shared.Entity;
using Newtonsoft.Json;

namespace FiveLife.Client.Login
{
    class SelectCharacter : FiveLifeScript
    {
        private bool IsCharacterSelectionActive = false;

        private CharacterCreator.CharacterCreator characterCreator = new CharacterCreator.CharacterCreator();
        private Vector3 pos = new Vector3(410.1618f, -2105.791f, 200f);
        Camera.Camera cam;
        private Vector3 pedPos = new Vector3(410.1618f, -2105.791f, 200f);
        private int lookAt = 0;

        private Dictionary<Ped, Character> peds = new Dictionary<Ped, Character>();

        private readonly Scaleform _instructionalButtonsScaleform = new Scaleform("instructional_buttons");
        private readonly List<InstructionalButton> _instructionalButtons = new List<InstructionalButton>();

        public override async void Initialize()
        {
            API.SetNuiFocus(false, false);
            API.RegisterNuiCallbackType("character_finished");
            EventHandlers.Add("__cfx_nui:character_finished", new Action<ExpandoObject>(CharacterFinished));

            API.ShutdownLoadingScreenNui();
            API.ShutdownLoadingScreen();

            // characterCreator.OnFinished += 

            await CitizenFX.Core.Game.Player.SetModel(PedHash.Michael);
            await CitizenFX.Core.Game.Player.Spawn(pos, 23, false);

            Screen.Hud.IsRadarVisible = false;

            Screen.Fading.FadeIn(1);
            while (Screen.Fading.IsFadingIn)
                await Delay(0);

            Debug.WriteLine($"Characters: {Game.Data.Player.Characters.Count}");
            if (Game.Data.Player.Characters.Count == 0)
            {
                await characterCreator.Start();

                Screen.Fading.FadeIn(2500);
                while (Screen.Fading.IsFadingIn)
                    await BaseScript.Delay(0);

                return;
            }

            _instructionalButtons.Add(new InstructionalButton(Control.Context, "Select"));
            _instructionalButtons.Add(new InstructionalButton(Control.InteractionMenu, "New Character"));

            _instructionalButtons.Add(new InstructionalButton(Control.MoveRightOnly, "Next"));
            _instructionalButtons.Add(new InstructionalButton(Control.MoveLeftOnly, "Previous"));

            UpdateScaleform();
            IsCharacterSelectionActive = true;

            cam = new Camera.Camera();
            cam.Position = new Vector3(412.3085f, -2109.825f, 201.6868f);
            cam.Enabled = true;

            foreach (var character in Game.Data.Player.Characters)
            {
                var model = new Model((PedHash)character.ModelHash);
                model.Request();
                while (!model.IsLoaded) await Delay(0);
                var ped = await World.CreatePed(model, pedPos, 0);
                await ped.Apply(character);
                peds.Add(ped, character);
            }

        }

        private async void CharacterFinished(dynamic _data)
        {
            var data = JsonConvert.SerializeObject(_data);
            var character = JsonConvert.DeserializeObject<Character>(data) as Character;

            // upload character data to server
            characterCreator.Close();

            // Get ingame
            CharacterSelected(character);

        }

            public void UpdateScaleform()
        {
            _instructionalButtonsScaleform.CallFunction("CLEAR_ALL");
            _instructionalButtonsScaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            _instructionalButtonsScaleform.CallFunction("CREATE_CONTAINER");
            int count = 0;
            foreach (var button in _instructionalButtons)
            {
                _instructionalButtonsScaleform.CallFunction("SET_DATA_SLOT", count, button.GetButtonId(), button.Text);
                count++;
            }
            _instructionalButtonsScaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
        }

        private async void CreateNewCancel()
        {

        }

        private async void CharacterSelected(Character obj)
        {
            Debug.WriteLine("{0}", $"Character selected! {obj.FirstName} {obj.LastName}");

            Screen.Fading.FadeOut(10);
            while (Screen.Fading.IsFadingOut)
                await Delay(0);

            NUI.Close();

            if(cam != null)
                cam.Enabled = false;

            IsCharacterSelectionActive = false;

            foreach (var ped in peds.Keys)
            {
                ped.Model.MarkAsNoLongerNeeded();
                ped.Delete();
            }

            showAll();

            FireEvent("fivelife.character.selected", obj);
            FireServerEvent("fivelife.character.selected", obj);
        }

        private void Draw(string text, System.Drawing.Color color, Vector3 position, int ya, bool center, int font)
        {
            var pos = Screen.WorldToScreen(position);
            var posx = pos.X / 1280;
            var posy = pos.Y / 1080;

            if (posx == 0 || posy == 0)
                return;

            API.SetTextColour(color.R, color.G, color.B, color.A);
            API.SetTextFont(font);
            API.SetTextScale(0.3f, 0.3f);
            API.SetTextWrap(0.0f, 1.0f);
            API.SetTextCentre(center);
            API.SetTextDropshadow(1, 0, 0, 0, 112);
            API.SetTextEdge(1, 0, 0, 0, 205);
            API.SetTextEntry("STRING");
            API.AddTextComponentString(text);
            Function.Call((Hash)0x61BB1D9B3A95D802, ya);
            API.DrawText(posx, posy);
        }

        public override async Task Loop()
        {
            if (characterCreator.Active)
            {
                characterCreator.Update();
                return;
            }

            if (!IsCharacterSelectionActive) return;

            if (cam != null)
                cam.Update();

            CitizenFX.Core.Game.Player.Character.Position = pos;

            Function.Call(Hash.DRAW_SCALEFORM_MOVIE_FULLSCREEN, _instructionalButtonsScaleform.Handle, 255, 255, 255, 255, 0);

            hideAll();

            if (peds.Count <= 0) return;

            float angle = -45;
            float radius = 3;
            foreach (var ped in peds)
            {
                ped.Key.Show();

                var offset = new Vector3(cam.Position.X + radius * (float)Math.Sin((angle / 180) * Math.PI), cam.Position.Y + radius * (float)Math.Cos((angle / 180) * Math.PI), pos.Z);
                ped.Key.Position = offset;

                var textPos = new Vector3(offset.X, offset.Y, offset.Z + 1.65f);
                Draw($"{ped.Value.FirstName} {ped.Value.FirstName}", System.Drawing.Color.FromArgb(255, 255, 255, 255), textPos, 1, true, 4);

                var a = new Vector2(cam.Position.X, cam.Position.Y);
                var b = new Vector2(offset.X, offset.Y);
                var dir = a - b;
                var heading = ((Math.Atan2(dir.Y, dir.X) * 180) / Math.PI) - 90;

                ped.Key.Heading = (float)heading;

                angle += 90 / peds.Count;
            }

            if (cam.Rotation == Vector3.Zero)
            {
                cam.MoveTo(new Camera.Waypoint(cam.Position, peds.Keys.ToArray()[lookAt].Position, 50, 0.5f));
                peds.Keys.ToArray()[lookAt].Task.HandsUp(-1);
            }

            if (CitizenFX.Core.Game.IsControlJustReleased(0, Control.MoveLeftOnly))
            {
                lookAt--;
                if (lookAt < 0)
                    lookAt = peds.Count - 1;

                foreach (var ped in peds)
                {
                    ped.Key.Task.ClearAll();
                }

                cam.MoveTo(new Camera.Waypoint(cam.Position, peds.Keys.ToArray()[lookAt].Position, 50, 0.5f));
            }

            if (CitizenFX.Core.Game.IsControlJustReleased(0, Control.MoveRightOnly))
            {
                lookAt++;
                if (lookAt > peds.Count - 1)
                    lookAt = 0;

                foreach (var ped in peds)
                {
                    ped.Key.Task.ClearAll();
                }

                cam.MoveTo(new Camera.Waypoint(cam.Position, peds.Keys.ToArray()[lookAt].Position, 50, 0.5f));
            }

            if (CitizenFX.Core.Game.IsControlJustReleased(0, Control.Context))
            {
                var character = peds.Values.ToArray()[lookAt];
                CharacterSelected(character);
                return;
            }

            if (CitizenFX.Core.Game.IsControlJustReleased(0, Control.InteractionMenu))
            {
                foreach (var ped in peds.Keys)
                {
                    ped.Delete();
                }

                await characterCreator.Start();
                return;
            }

            var selectedPed = peds.Keys.ToArray()[lookAt];
            if (selectedPed.Exists() && !selectedPed.IsDead)
            {
                if (!API.GetIsTaskActive(selectedPed.Handle, 0))
                {
                    selectedPed.Task.HandsUp(-1);
                }
            }
        }

        private void hideAll()
        {
            // Hide our playable ped
            CitizenFX.Core.Game.Player.Character.Hide();

            // Hide all player peds
            foreach (var player in new PlayerList())
                player.Character.Hide();

            // Hide all NPC peds
            foreach (var ped in new Game.PedsPool())
                ped.Hide();

        }

        private void showAll()
        {
            // Show our player ped
            CitizenFX.Core.Game.Player.Character.Show();

            // Show all player peds
            foreach (var player in new PlayerList())
                player.Character.Show();

            // Show all NPC peds
            foreach (var ped in new Game.PedsPool())
                ped.Show();
        }

    }
}
