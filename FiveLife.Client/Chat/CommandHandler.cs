using CitizenFX.Core;
using CitizenFX.Core.Native;
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
            API.RegisterCommand("fix", new Action(Fix), true);
            API.RegisterCommand("giveall", new Action<int, List<object>, string>(GiveWeapon), true);
            API.RegisterCommand("weather", new Action<int, List<object>, string>(AdjustWeather), true);

            API.RegisterCommand("extra", new Action<int, List<object>, string>(Extra), false);
            API.RegisterCommand("livery", new Action<int, List<object>, string>(Livery), false);
            API.RegisterCommand("taxi", new Action(Taxi), false);
        }

        private void Fix()
        {
            var veh = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (veh == null) return;

            veh.Repair();
            veh.ClearLastWeaponDamage();
        }

        private void Taxi()
        {
            var vehicle = CitizenFX.Core.Game.Player.Character.CurrentVehicle;
            if (vehicle == null) return;

            API.SetTaxiLights(vehicle.Handle, !vehicle.IsTaxiLightOn);
            Debug.WriteLine($"Taxi: {vehicle.IsTaxiLightOn.ToString()}");
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
            foreach(var weapon in Enum.GetValues(typeof(WeaponHash)))
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

            var veh = await World.CreateVehicle(model, CitizenFX.Core.Game.Player.Character.Position, CitizenFX.Core.Game.Player.Character.Heading);

            if (args.Count > 1)
            {
                var livery = Int32.Parse((string)args[1]);
                veh.Mods.Livery = Math.Min(livery, veh.Mods.LiveryCount);
            }

            CitizenFX.Core.Game.Player.Character.SetIntoVehicle(veh, VehicleSeat.Driver);


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

                await CitizenFX.Core.Game.Player.Spawn(new Vector3(x, y, z), 0, false);
            }
            else
            {
                var x = World.GetWaypointBlip().Position.X;
                var y = World.GetWaypointBlip().Position.Y;

                await CitizenFX.Core.Game.Player.Spawn(new Vector3(x, y, -199), 0, false); // todo: fix z
            }
        }


        private void Pos()
        {
            Debug.WriteLine("{0}", $"Current Position: {CitizenFX.Core.Game.Player.Character.Position.ToString()}");
            Debug.WriteLine("{0}", $"Current Heading : {CitizenFX.Core.Game.Player.Character.Heading}");
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
