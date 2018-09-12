using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using FiveLife.Shared.Entity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator
{
    public class CharacterCreator
    {
        public delegate void Finish(Character character);
        public event Finish OnFinish;

        public bool Active = false;

        Camera.Camera camera;
        Menu.Menu menu;

        #region Police locker room
        private Vector3 CameraPosition = new Vector3(x: 455.8541f, y: -991.0778f, z: 31.10116f);
        private Vector3 CameraViewOffset = new Vector3(0, 0, 0.08f);
        private float CameraFieldOfView = 40;
        private float zoom = 3;
        //private Vector3 FaceCameraPosition = new Vector3(x: 455.4531f, y: -991.0778f, z: 31.33116f);
        //private Vector3 FaceCameraRotation = new Vector3(x: 0f, y: 0, z: 25.83904f);
        //private float FaceCameraFieldOfView = 10;

        private Vector3 PlayerPosition = new Vector3(x: 454.2021f, y: -988.5843f, z: 31.38958f);
        private float PlayerHeading = 207.0087f;

        //private Vector3 lookat = new Vector3(x: 456.6599f, y: -987.7852f, z: 31.39453f);
        #endregion

        public CharacterCreator()
        {
            Game.Data.Character = new Character();
            Game.Data.Character.Player = Game.Data.Player;
            Game.Data.Character.HeadShapeMix = 0.50f;

        }

        private float angle = 145.2505f;

        public async Task Start()
        {
            Active = true;
            await CitizenFX.Core.Game.Player.Spawn(PlayerPosition, PlayerHeading);
            await BaseScript.Delay(500);

            Game.Data.Character.ModelHash = (long)PedHash.FreemodeMale01;

            await CitizenFX.Core.Game.Player.SetModel(PedHash.FreemodeMale01);
            CitizenFX.Core.Game.Player.Character.Style.SetDefaultClothes();

            Function.Call(Hash.SET_PLAYER_CONTROL, CitizenFX.Core.Game.Player, false, 1280);
            Function.Call(Hash.SET_ENTITY_COLLISION, CitizenFX.Core.Game.Player.Character, true);
            Function.Call(Hash.FREEZE_ENTITY_POSITION, CitizenFX.Core.Game.Player, true);
            Function.Call(Hash.SET_PLAYER_INVINCIBLE, CitizenFX.Core.Game.Player, true);

            Screen.Hud.IsVisible = false;
            Screen.Hud.IsRadarVisible = false;
            Screen.Effects.Stop();
            Screen.Effects.Start(ScreenEffect.SwitchHudOut);

            CitizenFX.Core.Game.Player.Character.Show();

            camera = new Camera.Camera(CameraPosition, Vector3.Zero, CameraFieldOfView);
            camera.Enabled = true;

            menu = new Menu.Menu();

            menu.Open();

        }

        public void Update()
        {
            if (camera == null) return;

            if (API.IsPauseMenuActive()) return;

            if (menu != null && menu.IsOpen())
            {
                menu.Update();
            }

            if(menu != null && !menu.IsOpen())
            {
                menu.Open();
            }

            if(CitizenFX.Core.Game.IsControlPressed(0, Control.CursorScrollUp))
                zoom -= 0.15f;

            if (CitizenFX.Core.Game.IsControlPressed(0, Control.CursorScrollDown))
                zoom += 0.15f;

            zoom = Math.Min(zoom, 3);
            zoom = Math.Max(zoom, 0.75f);

            float mouseX = -CitizenFX.Core.Game.GetDisabledControlNormal(0, Control.LookLeft);
            float mouseY = -CitizenFX.Core.Game.GetDisabledControlNormal(0, Control.LookDown);

            var cam = CitizenFX.Core.Game.Player.Character;

            angle -= mouseX * CitizenFX.Core.Game.LastFrameTime * 400;

            angle = Math.Min(angle, 290);
            angle = Math.Max(angle, 95);

            CameraPosition = new Vector3(cam.Position.X + zoom * (float)Math.Sin((angle / 180) * Math.PI), cam.Position.Y + zoom * (float)Math.Cos((angle / 180) * Math.PI), CameraPosition.Z);

            CameraPosition.Z -= mouseY * CitizenFX.Core.Game.LastFrameTime * 10;
            CameraPosition.Z = Math.Min(CameraPosition.Z, 31.4f);
            CameraPosition.Z = Math.Max(CameraPosition.Z, 29.888f);

            var lookAt = CitizenFX.Core.Game.Player.Character.Position;
            lookAt.Z = CameraPosition.Z;

            camera.Position = CameraPosition;
            camera.LookAt(lookAt);
            camera.Update();

        }

        public void Close()
        {
            Active = false;
            menu.Close();
            camera.Enabled = false;
        }
    }
}
