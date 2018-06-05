using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Chat
{
    public class CommandHandler : FiveLifeScript
    {

        public override void Initialize()
        {
            API.RegisterCommand("revive", new Action(Revive), true);
            API.RegisterCommand("heal", new Action<int, List<object>, string>(Heal), true);
            API.RegisterCommand("pos", new Action(Pos), true);
            API.RegisterCommand("teleport", new Action<int, List<object>, string>(Teleport), true);
            API.RegisterCommand("tp", new Action<int, List<object>, string>(Teleport), true);
            API.RegisterCommand("spawn", new Action<int, List<object>, string>(Spawn), true);
            API.RegisterCommand("giveall", new Action<int, List<object>, string>(GiveWeapon), true);
            API.RegisterCommand("weather", new Action<int, List<object>, string>(AdjustWeather), true);
            API.RegisterCommand("opacity", new Action<int, List<object>, string>(Opacity), true);
            API.RegisterCommand("hide", new Action(Hide), true);


            API.RegisterCommand("fix", new Action(Fix), false);
            API.RegisterCommand("extra", new Action<int, List<object>, string>(Extra), false);
            API.RegisterCommand("livery", new Action<int, List<object>, string>(Livery), false);
            API.RegisterCommand("test", new Action(Test), false);
            API.RegisterCommand("license", new Action(License), false);

        }

        private void Hide()
        {
            CitizenFX.Core.Game.Player.Character.IsVisible =
                !CitizenFX.Core.Game.Player.Character.IsVisible;
        }

        private void Opacity(int arg1, List<object> arg2, string arg3)
        {
            var value = Int32.Parse((string)arg2[0]);

            CitizenFX.Core.Game.Player.Character.Opacity = value;
        }

        private void License()
        {
            var veh = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (veh == null) return;

            Debug.WriteLine($"{veh.Mods.LicensePlate}");
        }

        private void Fix()
        {
            var veh = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (veh == null) return;

            veh.Repair();
            veh.ClearLastWeaponDamage();
            Debug.WriteLine($"Dirt level: {veh.DirtLevel}");
            veh.DirtLevel = 0;
        }

        private async void Test()
        {
            var playerPed = CitizenFX.Core.Game.Player.Character;
            var veh = playerPed.CurrentVehicle;

            if (veh == null) return;

            var bone = veh.Bones["numberplate"];
            Debug.WriteLine($"Numberplate: {(bone.IsValid ? bone.Position.ToString() : "INVALID")}"); ;


            //var playerPed = CitizenFX.Core.Game.Player.Character;
            //var switchToCoords = new Vector3(129.063f, 6616.838f, 31.827f);
            //var switchToModel = new Model(PedHash.FilmDirector);
            //var currentPos = CitizenFX.Core.Game.Player.Character.Position;
            //var switchType = API.GetIdealPlayerSwitchType(currentPos.X, currentPos.Y, currentPos.Z, switchToCoords.X, switchToCoords.Y, switchToCoords.Z);
            //var switchFlag = 1024;

            //if (switchType == 3)
            //{
            //    switchType = 2;
            //    if (switchToCoords.DistanceToSquared(currentPos) < 40)
            //    {
            //        Debug.WriteLine("Too close?!");
            //        return;
            //    }
            //}

            //switchToModel.Request();
            //while (!switchToModel.IsLoaded)
            //    await Delay(0);

            //var switchToPed = await World.CreatePed(switchToModel, switchToCoords, 0);
            //switchToPed.IsVisible = false;
            //switchToPed.IsInvincible = true;
            //API.SetEntityAsMissionEntity(switchToPed.Handle, true, false);
            //switchToPed.Task.ClearAllImmediately();

            //if (!playerPed.IsInjured)
            //    API.SetPedDesiredHeading(switchToPed.Handle, playerPed.Heading);

            //switchToPed.IsCollisionEnabled = false;
            //switchToPed.IsVisible = false;

            //API.StartPlayerSwitch(playerPed.Handle, switchToPed.Handle, switchFlag, switchType);

            //while (API.GetPlayerSwitchState() != 8)
            //    await Delay(0);

            //API.SetFocusEntity(switchToPed.Handle);
            //API.SetEntityCoords(playerPed.Handle, switchToCoords.X, switchToCoords.Y, switchToCoords.Z, false, false, false, false);
            //switchToPed.Delete();
        }

        private void AdjustWeather(int arg1, List<object> arg2, string arg3)
        {

            World.TransitionToWeather(World.NextWeather, 2);

        }

        private void Extra(int arg1, List<object> arg2, string arg3)
        {
            var vehicle = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (vehicle == null) return;
            if (arg2.Count < 2) return;

            var index = int.Parse((string)arg2[0]);
            var toggle = bool.Parse((string)arg2[1]);

            Debug.WriteLine($"{index}: {vehicle.ExtraExists(index)}");

            vehicle.ToggleExtra(index, toggle);
        }

        private void Livery(int arg1, List<object> arg2, string arg3)
        {
            var vehicle = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (vehicle == null) return;
            if (arg2.Count < 1) return;

            vehicle.Mods.Livery = int.Parse((string)arg2[0]);
            Debug.WriteLine($"{vehicle.Mods.LiveryCount}");
            Debug.WriteLine($"{vehicle.Mods.LocalizedLiveryName}");
        }

        private void GiveWeapon(int arg1, List<object> arg2, string arg3)
        {
            foreach (var weapon in Enum.GetValues(typeof(WeaponHash)))
            {
                CitizenFX.Core.Game.Player.Character.Weapons.Give((WeaponHash)weapon, 999, false, true);
            }
        }

        private async void Spawn(int source, List<object> args, string raw)
        {
            var model = new Model((string)args[0]);
            model.Request();


            while (!model.IsLoaded)
                await BaseScript.Delay(0);

            if (CitizenFX.Core.Game.Player.Character.CurrentVehicle != null)
                CitizenFX.Core.Game.Player.Character.CurrentVehicle.Delete();
            try
            {
                var veh = await World.CreateVehicle(model, CitizenFX.Core.Game.Player.Character.Position, CitizenFX.Core.Game.Player.Character.Heading);


                if (args.Count > 1)
                {
                    var livery = Int32.Parse((string)args[1]);
                    veh.Mods.Livery = Math.Min(livery, veh.Mods.LiveryCount);
                }

                CitizenFX.Core.Game.Player.Character.SetIntoVehicle(veh, VehicleSeat.Driver);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



            // var v = SCharacter.Character.Vehicles.Where(s => s.Model == (uint)model.Hash).FirstOrDefault();

            //if (v != null)
            //{
            //    var vehicles = new VehiclesPool();
            //    var existingInWorld = vehicles.Where(a => a.Mods.LicensePlate == v.LicensePlate).FirstOrDefault();
            //    if (existingInWorld != null)
            //    {
            //        existingInWorld.Delete();
            //    }

            //    veh.Mods.LicensePlate = v.LicensePlate;
            //}
            //else
            //{
            //    SCharacter.Character.Vehicles.Add(new EntityStore.EVehicle()
            //    {
            //        LicensePlate = veh.Mods.LicensePlate,
            //        Model = model.Hash
            //    });
            //}

        }


        private async void Teleport(int source, List<object> args, string raw)
        {
            if (args.Count < 3 && World.GetWaypointBlip() == null)
            {
                Debug.WriteLine("teleporting requires 3 arguments (x,y,z)");
                return;
            }

            if (args.Count == 3)
            {
                var raw_x = ((string)args[0]).Trim(new char[] { ' ', ',' });
                var raw_y = ((string)args[1]).Trim(new char[] { ' ', ',' });
                var raw_z = ((string)args[2]).Trim(new char[] { ' ', ',' });

                Debug.WriteLine("Teleporting to {0}:{1}:{2}", raw_x, raw_y, raw_z);

                var x = float.Parse(raw_x);
                var y = float.Parse(raw_y);
                var z = float.Parse(raw_z);

                await CitizenFX.Core.Game.Player.Teleport(new Vector3(x, y, z), 0);
            }
            else
            {
                var x = World.GetWaypointBlip().Position.X;
                var y = World.GetWaypointBlip().Position.Y;

                await CitizenFX.Core.Game.Player.Teleport(new Vector3(x, y, -199), 0); // todo: fix z
            }
        }


        private void Pos()
        {
            Debug.WriteLine("{0}", $"Current Position: {CitizenFX.Core.Game.Player.Character.Position.ToString()}");
            Debug.WriteLine("{0}", $"Current Heading : {CitizenFX.Core.Game.Player.Character.Heading}");

            var Position = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
            var Rotation = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_ROT, 2);
            Debug.WriteLine("{0}", $"Current Position: {Position}");
            Debug.WriteLine("{0}", $"Current Heading : {Rotation}");

        }

        private void Revive()
        {
            CitizenFX.Core.Game.Player.Character.Resurrect();
            CitizenFX.Core.Game.Player.Character.Health = 100;
            CitizenFX.Core.Game.Player.Character.ClearBloodDamage();
            CitizenFX.Core.Game.Player.Character.ClearLastWeaponDamage();
            CitizenFX.Core.Game.Player.Character.ResetVisibleDamage();
            CitizenFX.Core.Game.Player.Character.CancelRagdoll();
            CitizenFX.Core.Game.Player.Freeze(false);
        }

        private void Heal(int source, List<object> args, string raw)
        {
            CitizenFX.Core.Game.Player.Character.Health += int.Parse((string)args[0]);
        }

    }
}
