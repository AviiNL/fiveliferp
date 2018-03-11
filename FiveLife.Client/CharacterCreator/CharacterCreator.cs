using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.CharacterCreator
{
    public class CharacterCreator
    {

        Camera.Camera camera;
        Menu.Menu menu;

        #region Police locker room
        private Vector3 CameraPosition = new Vector3(x: 455.8541f, y: -991.0778f, z: 31.10116f);
        private Vector3 CameraViewOffset = new Vector3(0, 0, 0);
        private float CameraFieldOfView = 40;

        //private Vector3 FaceCameraPosition = new Vector3(x: 455.4531f, y: -991.0778f, z: 31.33116f);
        //private Vector3 FaceCameraRotation = new Vector3(x: 0f, y: 0, z: 25.83904f);
        //private float FaceCameraFieldOfView = 10;

        private Vector3 PlayerPosition = new Vector3(x: 454.2021f, y: -988.5843f, z: 31.38958f);
        private float PlayerHeading = 207.0087f;

        //private Vector3 lookat = new Vector3(x: 456.6599f, y: -987.7852f, z: 31.39453f);
        #endregion

        public CharacterCreator()
        {
            
        }

        public async Task Start()
        {

            await CitizenFX.Core.Game.Player.Spawn(PlayerPosition, PlayerHeading);

            Function.Call(Hash.SET_PLAYER_CONTROL, CitizenFX.Core.Game.Player, false, 1280);
            Function.Call(Hash.SET_ENTITY_COLLISION, CitizenFX.Core.Game.Player.Character, true);
            Function.Call(Hash.FREEZE_ENTITY_POSITION, CitizenFX.Core.Game.Player, true);
            Function.Call(Hash.SET_PLAYER_INVINCIBLE, CitizenFX.Core.Game.Player, true);

            Screen.Hud.IsVisible = false;
            Screen.Hud.IsRadarVisible = false;
            Screen.Effects.Stop();
            Screen.Effects.Start(ScreenEffect.SwitchHudOut);

            camera = new Camera.Camera(CameraPosition, Vector3.Zero, CameraFieldOfView);
            camera.Enabled = true;

            menu = new Menu.Menu();
            menu.Open();
        }

        public void Update()
        {
            if (camera != null)
            {
                camera.LookAt(new Vector3(0, -1, 0) + CitizenFX.Core.Game.Player.Character.Position + CameraViewOffset);
                camera.Update();
            }

            if(menu != null && menu.IsOpen())
            {
                menu.Update();
            }

            
        }
    }
}
