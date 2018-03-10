using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenFX.Core
{
    public static class PlayerExtension
    {
        public static void Freeze(this Player self, bool freeze = true)
        {
            Function.Call(Hash.SET_PLAYER_CONTROL, self, !freeze, 1280);

            var ped = self.Character;

            if (!freeze)
            {
                if (!Function.Call<bool>(Hash.IS_ENTITY_VISIBLE, ped))
                {
                    Function.Call(Hash.SET_ENTITY_VISIBLE, ped, true);
                }

                if (!Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, ped))
                {
                    Function.Call(Hash.SET_ENTITY_COLLISION, ped, true);
                }

                Function.Call(Hash.FREEZE_ENTITY_POSITION, self, false);
                Function.Call(Hash.SET_PLAYER_INVINCIBLE, self, false);
            }
            else
            {
                if (Function.Call<bool>(Hash.IS_ENTITY_VISIBLE, ped))
                {
                    Function.Call(Hash.SET_ENTITY_VISIBLE, ped, false);
                }

                Function.Call(Hash.SET_ENTITY_COLLISION, ped, false);
                Function.Call(Hash.FREEZE_ENTITY_POSITION, self, true);
                Function.Call(Hash.SET_PLAYER_INVINCIBLE, self, true);

                if (!Function.Call<bool>(Hash.IS_PED_FATALLY_INJURED, ped))
                {
                    Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, ped);
                }
            }

        }

        private static bool spawnLock = false;

        public static async Task Spawn(this Player self, FiveLife.Shared.Entity.Character obj)
        {
            self.Character.Apply(obj);

            self.SetStat("STAMINA", obj.Stamina);
            self.SetStat("STEALTH_ABILITY", obj.Stealth);
            self.SetStat("LUNG_CAPACITY", obj.LungCapacity);
            self.SetStat("FLYING_ABILITY", obj.Flying);
            self.SetStat("SHOOTING_ABILITY", obj.Shooting);
            self.SetStat("STRENGTH", obj.Strength);
            self.SetStat("WHEELIE_ABILITY", obj.Wheelie);

            await self.Spawn(new Vector3(obj.X, obj.Y, obj.Z), obj.Heading, true);
        }

        public static async Task Teleport(this Player self, Vector3 Position, float Heading)
        {
            //self.Character.FadeOut();

            Screen.Fading.FadeOut(200);
            while (Screen.Fading.IsFadingOut)
                await BaseScript.Delay(0);

            self.Character.PositionNoOffset = Position;
            self.Character.Heading = Heading;
            
            Screen.Fading.FadeIn(200);
            while (Screen.Fading.IsFadingIn)
                await BaseScript.Delay(0);

            //self.Character.FadeIn();

        }

        public static void SetStat(this Player self, string stat, int value)
        {
            Function.Call(Hash.STAT_SET_INT, Function.Call<uint>(Hash.GET_HASH_KEY, $"MP0_{stat}"), value, true);
            Function.Call(Hash.STAT_SET_INT, Function.Call<uint>(Hash.GET_HASH_KEY, $"SP0_{stat}"), value, true);
            Function.Call(Hash.STAT_SET_INT, Function.Call<uint>(Hash.GET_HASH_KEY, $"SP1_{stat}"), value, true);
            Function.Call(Hash.STAT_SET_INT, Function.Call<uint>(Hash.GET_HASH_KEY, $"SP2_{stat}"), value, true);
        }

        public static async Task Spawn(this Player self, Vector3 Position, float Heading = 0, bool removeWeapons = true)
        {
            if (spawnLock)
                return;

            spawnLock = true;

            self.Freeze(true);

            var ped = self.Character;

            Function.Call(Hash.SET_ENTITY_COORDS, ped, Position.X, Position.Y, Position.Z, false, false, false, true);

            Function.Call(Hash.REQUEST_COLLISION_AT_COORD, Position.X, Position.Y, Position.Z);

            while (!Function.Call<bool>(Hash.HAS_COLLISION_LOADED_AROUND_ENTITY, Game.Player.Character))
                await BaseScript.Delay(0);

            Game.Player.Character.CancelRagdoll();
            Function.Call(Hash.NETWORK_RESURRECT_LOCAL_PLAYER, Position.X, Position.Y, Position.Z, Heading, true, true, false);
            Function.Call(Hash.CLEAR_PED_TASKS_IMMEDIATELY, ped);

            if (removeWeapons)
                Function.Call(Hash.REMOVE_ALL_PED_WEAPONS, ped, true);

            Game.Player.Character.ClearBloodDamage();
            Game.Player.Character.ClearLastWeaponDamage();
            Game.Player.Character.ResetVisibleDamage();
            Game.Player.Character.IsInvincible = false;

            Function.Call(Hash.CLEAR_PLAYER_WANTED_LEVEL, self);
            Function.Call(Hash.SHUTDOWN_LOADING_SCREEN);

            self.Freeze(false);
            
            spawnLock = false;
        }

        public static async Task SetModel(this Player self, PedHash hash)
        {
            await self.SetModel(new Model(hash));
        }

        public static async Task SetModel(this Player self, Model model)
        {
            model.Request();
            if (!model.IsValid)
                return;

            while (!model.IsLoaded)
                await BaseScript.Delay(0);

            await Game.Player.ChangeModel(model);
            // Function.Call(Hash.SET_PLAYER_MODEL, self.Character, model.Hash);
            
        }
    }
}
