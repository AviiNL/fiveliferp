using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Shared.ELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.ELS
{
    // Converted from https://github.com/MrDaGree/ELS-FiveM/blob/master/client/client.lua
    public class ELS : FiveLifeScript
    {
        private float fps = 0; // still no clue as to why we need dis

        private Dictionary<int, ElsVehicle> elsVehs = new Dictionary<int, ElsVehicle>();
        private Dictionary<int, int> m_siren_state = new Dictionary<int, int>();
        private Dictionary<int, int> d_siren_state = new Dictionary<int, int>();
        private Dictionary<int, int> h_horn_state = new Dictionary<int, int>();
        private Dictionary<int, int> m_soundID_veh = new Dictionary<int, int>();
        private Dictionary<int, int> d_soundID_veh = new Dictionary<int, int>();
        private Dictionary<int, int> h_soundID_veh = new Dictionary<int, int>();
        private Dictionary<int, bool> dualEnable = new Dictionary<int, bool>();

        private Dictionary<int, bool> vehIsReady_stage2 = new Dictionary<int, bool>();
        private Dictionary<int, bool> vehIsReady_stage3 = new Dictionary<int, bool>();
        private Dictionary<int, bool> vehIsReady_advisor = new Dictionary<int, bool>();

        private Dictionary<string, Shared.ELS.ElsVehicle> els_Vehicles;
        private List<ElsState> Primaries;
        private List<ElsState> Secondaries;
        private List<ElsState> Advisors;

        private int advisorPatternSelectedIndex = 0;

        private int lightPatternPrim = 0;
        private int lightPatternSec = 0;

        private int lastVeh = 0;
        private int curCleanupTime = 0;
        private int stageThreeAllow = 1;
        private int stageTwoAllow = 1;
        private int advisorAllow = 1;

        private bool guiEnabled = true;
        private bool lastVehDmgReset = false;

        public override void Initialize()
        {
            RegisterEvent<Dictionary<string, Shared.ELS.ElsVehicle>, List<ElsState>, List<ElsState>, List<ElsState>>("fivelife.sync.els.request_vehicle_update", OnVehicleUpdate);

            RegisterEvent<int, int, int, int, int>("fivelife.sync.els.change_light_stage", OnChangeLightStage);
            RegisterEvent<int, int>("fivelife.sync.els.change_primary_pattern", OnChangePrimaryPattern);
            RegisterEvent<int, int>("fivelife.sync.els.change_secondary_pattern", OnChangeSecondaryPattern);
            RegisterEvent<int, int>("fivelife.sync.els.change_advisor_pattern", OnChangeAdvisorPattern);

            RegisterEvent<int, int>("fivelife.sync.els.set_siren_state", OnSetSirenState);
            RegisterEvent<int, bool>("fivelife.sync.els.set_dual_siren", OnSetDualSiren);
            RegisterEvent<int, int>("fivelife.sync.els.set_dual_siren_state", OnSetDualSirenState);
            RegisterEvent<int, int>("fivelife.sync.els.set_horn_state", OnSetHornState);
            RegisterEvent<int, int>("fivelife.sync.els.set_takedown_state", OnSetTakedownState);

            FireServerEvent("fivelife.sync.els.request_vehicle_update", CitizenFX.Core.Game.Player.ServerId, CitizenFX.Core.Game.Player.ServerId);
        }

        private async Task PatternStageThreeWorker()
        {
            foreach (var v in elsVehs)
            {
                if (API.DoesEntityExist(v.Key))
                {
                    if (v.Value.stage == 3)
                    {
                        var carLoc = API.GetEntityCoords(v.Key, true);
                        var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);
                        if (carLoc.DistanceToSquared(playerPos) <= Config.vehicleSyncDistance)
                        {
                            API.SetVehicleAutoRepairDisabled(v.Key, true);

                            if (!vehIsReady_stage3.ContainsKey(v.Key))
                                vehIsReady_stage3[v.Key] = true;

                            if (vehIsReady_stage3[v.Key])
                            {
                                runPatternStageThree(v.Key, v.Value.primPattern, new Action<bool>((ret) =>
                                {
                                    vehIsReady_stage3[v.Key] = ret;
                                }));
                            }
                        }
                    }
                }
            }
        }

        private async void runPatternStageThree(int key, int pattern, Action<bool> action)
        {
            if (API.DoesEntityExist(key) && !API.IsEntityDead(key))
            {
                API.SetVehicleAutoRepairDisabled(key, true);

                var max = Primaries[pattern].stages.Count;
                var count = 1;

                var lastSpeed = Primaries[pattern].speed;

                var rate = fps / (fps * 60 / lastSpeed);

                if (rate < 1)
                    rate = (float)Math.Ceiling(rate);
                else
                    rate = (float)Math.Floor(rate);

                if (rate == stageThreeAllow)
                {
                    stageThreeAllow = 1;

                    action.Invoke(false);

                    while (count <= max)
                    {
                        if (!API.DoesEntityExist(key) || API.IsEntityDead(key) || pattern != elsVehs[key].primPattern)
                            break;

                        for (int i = 1; i <= 12; i++)
                        {
                            if (Primaries[pattern].stages[count].ContainsKey(i.ToString()))
                            {
                                setExtraState(key, i, (Primaries[pattern].stages[count][i.ToString()] == 1));
                                if (Primaries[pattern].stages[count][i.ToString()] == 0)
                                {
                                    runEnvironmentLights(key, i);
                                }
                            }
                        }

                        if (Primaries[pattern].stages[count].ContainsKey("speed"))
                        {
                            lastSpeed = Primaries[pattern].stages[count]["speed"];
                        }

                        await Delay(lastSpeed);
                        count++;
                    }

                    action.Invoke(true);

                }
                else if (stageThreeAllow > rate)
                {
                    stageThreeAllow = 1;
                }
                else
                {
                    stageThreeAllow++;
                }

            }
        }

        private async void runEnvironmentLights(int key, int extra)
        {
            if (API.DoesEntityExist(key) && !API.IsEntityDead(key))
            {
                var vehN = checkCarHash(key);

                if (els_Vehicles[vehN].extras[extra].env_light)
                {
                    var boneIndex = API.GetEntityBoneIndexByName(key, $"extra_{extra}");
                    var coords = API.GetWorldPositionOfEntityBone(key, boneIndex);

                    if (API.IsVehicleExtraTurnedOn(key, extra))
                    {
                        API.DrawLightWithRange(coords.X + els_Vehicles[vehN].extras[extra].env_pos_x, coords.Y + els_Vehicles[vehN].extras[extra].env_pos_y, coords.Z + els_Vehicles[vehN].extras[extra].env_pos_z, els_Vehicles[vehN].extras[extra].env_color.R, els_Vehicles[vehN].extras[extra].env_color.G, els_Vehicles[vehN].extras[extra].env_color.B, Config.vehicleSyncDistance, Config.environmentLightBrightness);
                        await Delay(2);
                    }

                }
            }

        }

        // https://github.com/MrDaGree/ELS-FiveM/blob/master/client/client.lua#L1650
        private async Task PatternStageTwoWorker()
        {
            foreach (var v in elsVehs)
            {
                var elsVehicle = v.Key;

                if (API.DoesEntityExist(v.Key))
                {
                    if (v.Value.stage == 0)
                    {
                        setExtraState(elsVehicle, 1, true);
                        setExtraState(elsVehicle, 2, true);
                        setExtraState(elsVehicle, 3, true);
                        setExtraState(elsVehicle, 4, true);
                        setExtraState(elsVehicle, 5, true);
                        setExtraState(elsVehicle, 6, true);
                        setExtraState(elsVehicle, 7, true);
                        setExtraState(elsVehicle, 8, true);
                        setExtraState(elsVehicle, 9, true);
                        // setExtraState(elsVehicle, 10, true);
                        // setExtraState(elsVehicle, 11, true);
                        // setExtraState(elsVehicle, 12, true);
                    }
                    else if (v.Value.stage == 2 || v.Value.stage == 3)
                    {
                        var carLoc = API.GetEntityCoords(elsVehicle, true);
                        var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);
                        if (carLoc.DistanceToSquared(playerPos) <= Config.vehicleSyncDistance)
                        {
                            API.SetVehicleAutoRepairDisabled(elsVehicle, true);

                            if (!vehIsReady_stage2.ContainsKey(elsVehicle))
                            {
                                vehIsReady_stage2[elsVehicle] = true;
                            }

                            if (vehIsReady_stage2[v.Key])
                            {

                                runPatternStageTwo(v.Key, v.Value.secPattern, new Action<bool>((ret) =>
                                {
                                    vehIsReady_stage2[v.Key] = ret;
                                }));
                            }

                        }
                    }
                }
            }
        }

        private async void runPatternStageTwo(int key, int pattern, Action<bool> action)
        {
            if (API.DoesEntityExist(key) && !API.IsEntityDead(key))
            {
                API.SetVehicleAutoRepairDisabled(key, true);

                var max = Secondaries[pattern].stages.Count;
                var count = 1;

                var lastSpeed = Secondaries[pattern].speed;

                var rate = fps / (fps * 60 / lastSpeed);

                if (rate < 1)
                    rate = (float)Math.Ceiling(rate);
                else
                    rate = (float)Math.Floor(rate);

                if (rate == stageTwoAllow)
                {
                    stageTwoAllow = 1;
                    action.Invoke(false);

                    while (count <= max)
                    {
                        if (!API.DoesEntityExist(key) || API.IsEntityDead(key) || pattern != elsVehs[key].secPattern)
                            break;

                        for (int i = 1; i <= 12; i++)
                        {
                            if (doesVehicleHaveTrafficAdvisor(key))
                            {
                                if (i != 7 && i != 8 && i != 9)
                                {
                                    if (Secondaries[pattern].stages[count].ContainsKey(i.ToString()))
                                    {
                                        setExtraState(key, i, (Secondaries[pattern].stages[count][i.ToString()] == 1));
                                        if (Secondaries[pattern].stages[count][i.ToString()] == 0)
                                        {
                                            runEnvironmentLights(key, i);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Secondaries[pattern].stages[count].ContainsKey(i.ToString()))
                                {
                                    setExtraState(key, i, (Secondaries[pattern].stages[count][i.ToString()] == 1));
                                    if (Secondaries[pattern].stages[count][i.ToString()] == 0)
                                    {
                                        runEnvironmentLights(key, i);
                                    }
                                }
                            }
                        }

                        if (Secondaries[pattern].stages[count].ContainsKey("speed"))
                        {
                            lastSpeed = Secondaries[pattern].stages[count]["speed"];
                        }

                        await Delay(lastSpeed);
                        count++;
                    }

                    action.Invoke(true);

                }
                else if (stageTwoAllow > rate)
                {
                    stageTwoAllow = 1;
                }
                else
                {
                    stageTwoAllow++;
                }
            }
        }

        // https://github.com/MrDaGree/ELS-FiveM/blob/master/client/client.lua#L1585
        private async Task PatternAdvisorWorker()
        {
            foreach (var v in elsVehs)
            {
                if (API.DoesEntityExist(v.Key))
                {
                    if (doesVehicleHaveTrafficAdvisor(v.Key))
                    {
                        var carLoc = API.GetEntityCoords(v.Key, true);
                        var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);
                        if (carLoc.DistanceToSquared(playerPos) <= Config.vehicleSyncDistance)
                        {
                            API.SetVehicleAutoRepairDisabled(v.Key, true);

                            if (!vehIsReady_advisor.ContainsKey(v.Key))
                                vehIsReady_advisor[v.Key] = true;

                            if (vehIsReady_advisor[v.Key])
                            {
                                runPatternAdvisor(v.Key, v.Value.stage, v.Value.advisorPattern, new Action<bool>((ret) =>
                                {
                                    vehIsReady_advisor[v.Key] = ret;
                                }));
                            }
                        }
                    }
                }
            }
        }

        private async void runPatternAdvisor(int key, int stage, int pattern, Action<bool> action)
        {
            if (API.DoesEntityExist(key) && !API.IsEntityDead(key))
            {
                if (stage == 1 || stage == 2 || (canUseAdvisorStageThree(key) && stage == 3))
                {

                    API.SetVehicleAutoRepairDisabled(key, true);

                    var max = Advisors[pattern].stages.Count;
                    var count = 1;

                    var lastSpeed = Advisors[pattern].speed;

                    var rate = fps / (fps * 60 / lastSpeed);

                    if (rate < 1)
                        rate = (float)Math.Ceiling(rate);
                    else
                        rate = (float)Math.Floor(rate);

                    if (rate == advisorAllow)
                    {
                        advisorAllow = 1;
                        action.Invoke(false);

                        while (count <= max)
                        {
                            if (!API.DoesEntityExist(key) || API.IsEntityDead(key) || pattern != elsVehs[key].advisorPattern)
                                break;

                            for (int i = 1; i <= 12; i++)
                            {
                                if (Advisors[pattern].stages[count].ContainsKey(i.ToString()))
                                {
                                    setExtraState(key, i, (Advisors[pattern].stages[count][i.ToString()] == 1));
                                    if (Advisors[pattern].stages[count][i.ToString()] == 0)
                                    {
                                        runEnvironmentLights(key, i);
                                    }
                                }
                            }

                            if (Advisors[pattern].stages[count].ContainsKey("speed"))
                            {
                                lastSpeed = Advisors[pattern].stages[count]["speed"];
                            }

                            await Delay(lastSpeed);
                            count++;
                        }

                        action.Invoke(true);

                    }
                    else if (advisorAllow > rate)
                    {
                        advisorAllow = 1;
                    }
                    else
                    {
                        advisorAllow++;
                    }
                }
            }
        }

        private async Task ExtraWorker()
        {
            foreach (var v in elsVehs)
            {
                if (API.DoesEntityExist(v.Key))
                {
                    if (v.Value.stage == 0)
                    {
                        setExtraState(v.Key, 1, true);
                        setExtraState(v.Key, 2, true);
                        setExtraState(v.Key, 3, true);
                        setExtraState(v.Key, 4, true);
                        setExtraState(v.Key, 5, true);
                        setExtraState(v.Key, 6, true);
                        setExtraState(v.Key, 7, true);
                        setExtraState(v.Key, 8, true);
                        setExtraState(v.Key, 9, true);
                        //setExtraState(v.Key, 10, true);
                        //setExtraState(v.Key, 11, true);
                        //setExtraState(v.Key, 12, true);
                    }
                    else if (v.Value.stage == 1)
                    {
                        setExtraState(v.Key, 1, true);
                        setExtraState(v.Key, 2, true);
                        setExtraState(v.Key, 3, true);
                        setExtraState(v.Key, 4, true);
                        setExtraState(v.Key, 5, true);
                        setExtraState(v.Key, 6, true);
                    }
                    else if (v.Value.stage == 2)
                    {
                        setExtraState(v.Key, 1, true);
                        setExtraState(v.Key, 2, true);
                        setExtraState(v.Key, 3, true);
                        setExtraState(v.Key, 4, true);
                    }
                }
            }
        }

        private async Task LightWorker()
        {
            foreach (var v in elsVehs)
            {
                if (API.DoesEntityExist(v.Key))
                {
                    var carLoc = API.GetEntityCoords(v.Key, true);
                    var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);
                    if (carLoc.DistanceToSquared(playerPos) <= Config.vehicleSyncDistance)
                    {
                        var vehN = checkCarHash(v.Key);

                        for (int i = 1; i <= 12; i++)
                        {
                            if (!API.IsEntityDead(v.Key) && API.DoesEntityExist(v.Key))
                            {
                                if (els_Vehicles.ContainsKey(vehN) && els_Vehicles[vehN].extras.ContainsKey(i))
                                {
                                    if (API.IsVehicleExtraTurnedOn(v.Key, i))
                                    {
                                        var boneIndex = API.GetEntityBoneIndexByName(v.Key, $"extra_{i}");
                                        var coords = API.GetWorldPositionOfEntityBone(v.Key, boneIndex);
                                        var rot = RotAnglesToVec(API.GetEntityRotation(v.Key, 2));

                                        if (els_Vehicles[vehN].extras[i].env_light)
                                        {
                                            if (i == 11)
                                            {
                                                API.DrawSpotLightWithShadow(coords.X + els_Vehicles[vehN].extras[11].env_pos_x, coords.Y + els_Vehicles[vehN].extras[11].env_pos_y, coords.Z + els_Vehicles[vehN].extras[11].env_pos_z, rot.X, rot.Y, rot.Z, 255, 255, 255, 75.0f, 2.0f, 10.0f, 20.0f, 0.0f, 1);
                                            }
                                        }
                                        else
                                        {
                                            if (i == 11)
                                            {
                                                API.DrawSpotLightWithShadow(coords.X, coords.Y, coords.Z + 0.2f, rot.X, rot.Y, rot.Z, 255, 255, 255, 75.0f, 2.0f, 10.0f, 20.0f, 0.0f, 1);
                                            }
                                        }

                                        if (doesVehicleHaveTrafficAdvisor(v.Key))
                                        {
                                            if (i != 7 && i != 8 && i != 9)
                                            {
                                                runEnvironmentLightWithBrightness(v.Key, i, 0.01f);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await Delay(4);
        }

        private async void runEnvironmentLightWithBrightness(int key, int extra, float brightness)
        {
            if (API.DoesEntityExist(key) && !API.IsEntityDead(key))
            {
                var vehN = checkCarHash(key);

                if (els_Vehicles[vehN].extras[extra].env_light)
                {
                    var boneIndex = API.GetEntityBoneIndexByName(key, $"extra_{extra}");
                    var coords = API.GetWorldPositionOfEntityBone(key, boneIndex);

                    if (API.IsVehicleExtraTurnedOn(key, extra))
                    {
                        API.DrawLightWithRange(coords.X + els_Vehicles[vehN].extras[extra].env_pos_x, coords.Y + els_Vehicles[vehN].extras[extra].env_pos_y, coords.Z + els_Vehicles[vehN].extras[extra].env_pos_z, els_Vehicles[vehN].extras[extra].env_color.R, els_Vehicles[vehN].extras[extra].env_color.G, els_Vehicles[vehN].extras[extra].env_color.B, Config.vehicleSyncDistance, brightness);
                        await Delay(2);
                    }
                }
            }
        }

        private Vector3 RotAnglesToVec(Vector3 rot)
        {
            var z = Math.PI * rot.Z / 180.0;
            var x = Math.PI * rot.X / 180.0;
            var num = Math.Abs(Math.Cos(x));

            return new Vector3((float)(-Math.Sin(z) * num), (float)(Math.Cos(z) * num), (float)(Math.Sin(x)));
        }

        public override async Task Loop()
        {
            if (els_Vehicles == null) return;

            fps = CitizenFX.Core.Game.FPS;

            var vehId = API.GetVehiclePedIsUsing(API.GetPlayerPed(-1));

            if (els_Vehicles.ContainsKey(checkCarHash(vehId)))
            {
                if ((API.GetPedInVehicleSeat(vehId, -1) == API.GetPlayerPed(-1)) ||
                    (API.GetPedInVehicleSeat(vehId, 0) == API.GetPlayerPed(-1)))
                {
                    if (API.GetVehicleClass(vehId) == 18)
                    {
                        API.DisableControlAction(0, 86, true);
                    }

                    API.DisableControlAction(0, 84, true);//-- INPUT_VEH_PREV_RADIO_TRACK
                    API.DisableControlAction(0, 83, true);//-- INPUT_VEH_NEXT_RADIO_TRACK
                    API.DisableControlAction(0, 81, true);//-- INPUT_VEH_NEXT_RADIO
                    API.DisableControlAction(0, 82, true);//-- INPUT_VEH_PREV_RADIO
                    API.DisableControlAction(0, 85, true);//-- INPUT_VEH_PREV_RADIO

                    API.SetVehRadioStation(vehId, "OFF");
                    API.SetVehicleRadioEnabled(vehId, false);

                    if (API.GetLastInputMethod(0)) // Keyboard
                    {
                        API.DisableControlAction(0, (int)Config.Keyboard.stageChange, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.primary, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.secondary, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.advisor, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.modifyKey, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.tone_one, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.tone_two, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.tone_three, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.dual_toggle, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.dual_one, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.dual_two, true);
                        API.DisableControlAction(0, (int)Config.Keyboard.dual_three, true);

                        if (API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey))
                        {
                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.primary))
                            {
                                Sound();
                                var primMax = Primaries.Count - 1;
                                var primMin = 0;
                                var temp = lightPatternPrim;

                                temp = temp - 1;

                                if (temp < primMin)
                                {
                                    temp = primMax;
                                }

                                lightPatternPrim = temp;

                                changeLightPattern(lightPatternPrim);
                            }

                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.secondary))
                            {
                                Sound();
                                var primMax = Secondaries.Count - 1;
                                var primMin = 0;
                                var temp = lightPatternSec;

                                temp = temp - 1;

                                if (temp < primMin)
                                {
                                    temp = primMax;
                                }

                                lightPatternSec = temp;
                                changeSecondaryPattern(lightPatternSec);
                            }

                        }
                        else
                        {
                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.primary))
                            {
                                Sound();
                                var primMax = Primaries.Count - 1;
                                var primMin = 0;
                                var temp = lightPatternPrim;

                                temp = temp + 1;

                                if (temp > primMax)
                                {
                                    temp = primMin;
                                }

                                lightPatternPrim = temp;

                                changeLightPattern(lightPatternPrim);
                            }

                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.secondary))
                            {
                                Sound();
                                var primMax = Secondaries.Count - 1;
                                var primMin = 0;
                                var temp = lightPatternSec;

                                temp = temp + 1;

                                if (temp > primMax)
                                {
                                    temp = primMin;
                                }

                                lightPatternSec = temp;
                                changeSecondaryPattern(lightPatternSec);
                            }
                        }

                        if (doesVehicleHaveTrafficAdvisor(vehId))
                        {
                            if (API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey))
                            {
                                if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.advisor))
                                {
                                    Sound();
                                    var primMax = Advisors.Count - 1;
                                    var primMin = 0;
                                    var temp = advisorPatternSelectedIndex;

                                    temp = temp - 1;

                                    if (temp < primMin)
                                    {
                                        temp = primMax;
                                    }
                                    advisorPatternSelectedIndex = temp;

                                    changeAdvisorPattern(advisorPatternSelectedIndex);
                                }
                            }
                            else
                            {
                                if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.advisor))
                                {
                                    Sound();
                                    var primMax = Advisors.Count - 1;
                                    var primMin = 0;
                                    var temp = advisorPatternSelectedIndex;

                                    temp = temp + 1;

                                    if (temp > primMax)
                                    {
                                        temp = primMin;
                                    }
                                    advisorPatternSelectedIndex = temp;

                                    changeAdvisorPattern(advisorPatternSelectedIndex);
                                }
                            }
                        }

                        if (API.GetVehicleClass(vehId) == 18)
                        {
                            if (elsVehs.ContainsKey(vehId))
                            {
                                if (elsVehs[vehId].stage == 3)
                                {
                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_one))
                                    {
                                        Sound();

                                        if (!m_siren_state.ContainsKey(vehId) ||
                                            m_siren_state[vehId] == 2 ||
                                            m_siren_state[vehId] == 3 ||
                                            m_siren_state[vehId] == 0)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                        else if (m_siren_state[vehId] == 1)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        }
                                    }

                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_two))
                                    {
                                        Sound();

                                        if (!m_siren_state.ContainsKey(vehId) ||
                                            m_siren_state[vehId] == 1 ||
                                            m_siren_state[vehId] == 3 ||
                                            m_siren_state[vehId] == 0)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 2);
                                        }
                                        else if (m_siren_state[vehId] == 2)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        }
                                    }
                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_three))
                                    {
                                        Sound();

                                        if (!m_siren_state.ContainsKey(vehId) ||
                                            m_siren_state[vehId] == 1 ||
                                            m_siren_state[vehId] == 2 ||
                                            m_siren_state[vehId] == 0)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 3);
                                        }
                                        else if (m_siren_state[vehId] == 3)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        }
                                    }

                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.dual_toggle))
                                    {
                                        Sound();

                                        if (dualEnable.ContainsKey(vehId) && dualEnable[vehId])
                                        {
                                            FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, false);
                                            FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        }
                                        else
                                        {
                                            FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, true);
                                        }
                                    }

                                    if (dualEnable.ContainsKey(vehId) && dualEnable[vehId])
                                    {
                                        if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.dual_one))
                                        {
                                            Sound();
                                            FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                        if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.dual_two))
                                        {
                                            Sound();
                                            FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 2);
                                        }
                                        if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.dual_three))
                                        {
                                            Sound();
                                            FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 3);
                                        }
                                    }
                                }

                                if (elsVehs[vehId].stage == 2)
                                {
                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_one))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                    }
                                    if (API.IsDisabledControlJustPressed(0, (int)Config.Keyboard.tone_one))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                    }

                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_two))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                    }
                                    if (API.IsDisabledControlJustPressed(0, (int)Config.Keyboard.tone_two))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 2);
                                    }

                                    if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.tone_three))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                    }
                                    if (API.IsDisabledControlJustPressed(0, (int)Config.Keyboard.tone_three))
                                    {
                                        Sound();
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 3);
                                    }
                                }
                            }
                        }

                        if (API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey))
                        {
                            API.DisableControlAction(0, (int)Config.Keyboard.takedown, true);
                            API.DisableControlAction(0, (int)Config.Keyboard.takedown, true);

                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.guiKey))
                            {
                                Sound();
                                guiEnabled = !guiEnabled;
                            }

                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.takedown))
                            {
                                Sound();
                                FireServerEvent("fivelife.sync.els.set_takedown_state", CitizenFX.Core.Game.Player.ServerId, 0);
                            }
                        }

                        if (els_Vehicles[checkCarHash(vehId)].activateUp)
                        {

                            if (API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey) && API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.stageChange))
                            {
                                Sound();

                                var newStage = 3;

                                if (elsVehs.ContainsKey(vehId))
                                {
                                    newStage = elsVehs[vehId].stage - 1;
                                }

                                if (newStage == 1)
                                {
                                    if (!doesVehicleHaveTrafficAdvisor(vehId))
                                    {
                                        newStage = 0;
                                    }
                                }

                                if (newStage == -1)
                                {
                                    newStage = 3;
                                }

                                changeLightStage(newStage, advisorPatternSelectedIndex, lightPatternPrim, lightPatternSec);

                                if (API.GetVehicleClass(vehId) == 18)
                                {
                                    if (newStage == 3)
                                    {
                                        API.SetVehicleSiren(vehId, true);
                                        if (Config.stagethreewithsiren)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                    }
                                    else
                                    {
                                        API.SetVehicleSiren(vehId, false);
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, false);
                                    }
                                }
                            }
                            else if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.stageChange))
                            {
                                Sound();

                                var newStage = 1;

                                if (elsVehs.ContainsKey(vehId))
                                {
                                    newStage = elsVehs[vehId].stage + 1;
                                }

                                if (newStage == 1)
                                {
                                    if (!doesVehicleHaveTrafficAdvisor(vehId))
                                    {
                                        newStage = 2;
                                    }
                                }

                                if (newStage == 4)
                                {
                                    newStage = 0;
                                }

                                changeLightStage(newStage, advisorPatternSelectedIndex, lightPatternPrim, lightPatternSec);

                                if (API.GetVehicleClass(vehId) == 18)
                                {
                                    if (newStage == 3)
                                    {
                                        API.SetVehicleSiren(vehId, true);
                                        if (Config.stagethreewithsiren)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                    }
                                    else
                                    {
                                        API.SetVehicleSiren(vehId, false);
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.stageChange))
                            {
                                Sound();
                                var newStage = 3;

                                if (elsVehs.ContainsKey(vehId))
                                {
                                    newStage = elsVehs[vehId].stage - 1;
                                }

                                if (newStage == 1)
                                {
                                    if (!doesVehicleHaveTrafficAdvisor(vehId))
                                    {
                                        newStage = 0;
                                    }
                                }

                                if (newStage == -1)
                                    newStage = 3;

                                changeLightStage(newStage, advisorPatternSelectedIndex, lightPatternPrim, lightPatternSec);

                                if (API.GetVehicleClass(vehId) == 18)
                                {
                                    if (newStage == 3)
                                    {
                                        API.SetVehicleSiren(vehId, true);
                                        if (Config.stagethreewithsiren)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                    }
                                    else
                                    {
                                        API.SetVehicleSiren(vehId, false);
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, false);
                                    }
                                }

                            }
                            else if (API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey) && API.IsDisabledControlJustReleased(0, (int)Config.Keyboard.stageChange))
                            {
                                Sound();
                                var newStage = 1;

                                if (elsVehs.ContainsKey(vehId))
                                {
                                    newStage = elsVehs[vehId].stage + 1;
                                }

                                if (newStage == 1)
                                {
                                    if (!doesVehicleHaveTrafficAdvisor(vehId))
                                    {
                                        newStage = 2;
                                    }
                                }

                                if (newStage == 4)
                                {
                                    newStage = 0;
                                }

                                changeLightStage(newStage, advisorPatternSelectedIndex, lightPatternPrim, lightPatternSec);

                                if (API.GetVehicleClass(vehId) == 18)
                                {
                                    if (newStage == 3)
                                    {
                                        API.SetVehicleSiren(vehId, true);
                                        if (Config.stagethreewithsiren)
                                        {
                                            FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 1);
                                        }
                                    }
                                    else
                                    {
                                        API.SetVehicleSiren(vehId, false);
                                        FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
                                        FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, false);
                                    }
                                }
                            }
                        }
                    }
                    else // Controller
                    {
                        // TODO: Controller....
                    }

                    if (API.GetVehicleClass(vehId) == 18)
                    {
                        if (!API.IsDisabledControlPressed(0, (int)Config.Keyboard.modifyKey))
                        {
                            if (API.IsDisabledControlJustPressed(0, 86))
                            {
                                FireServerEvent("fivelife.sync.els.set_horn_state", CitizenFX.Core.Game.Player.ServerId, 1);
                            }

                            if (API.IsDisabledControlJustReleased(0, 86))
                            {
                                FireServerEvent("fivelife.sync.els.set_horn_state", CitizenFX.Core.Game.Player.ServerId, 0);
                            }
                        }
                    }


                }
            }
            else
            {
                lastVeh = vehId;
            }


            if (els_Vehicles.ContainsKey(checkCarHash(vehId)))
            {
                if (getVehicleLightStage(vehId) != 0)
                {
                    API.SetVehicleEngineOn(vehId, true, true, false);
                }
            }

            LghtSoundCleaner();

        }

        private int getVehicleLightStage(int vehId)
        {
            if (elsVehs.ContainsKey(vehId))
            {
                return elsVehs[vehId].stage;
            }

            return 0;
        }

        private void LghtSoundCleaner()
        {
            if (curCleanupTime > 350)
            {
                curCleanupTime = 0;

                vehicleLightCleanup();
                hornCleanup();
                sirenCleanup();
            }
            else
            {
                curCleanupTime++;
            }
        }

        private void sirenCleanup()
        {
            var removals_m = new List<int>();
            foreach (var v in m_siren_state)
            {
                if (v.Value >= 0)
                {
                    if (!API.DoesEntityExist(v.Key) || API.IsEntityDead(v.Key))
                    {
                        if (m_soundID_veh.ContainsKey(v.Key))
                        {
                            API.StopSound(m_soundID_veh[v.Key]);
                            API.ReleaseSoundId(m_soundID_veh[v.Key]);
                            removals_m.Add(v.Key);
                        }
                    }
                }
            }
            foreach (var r in removals_m)
            {
                m_soundID_veh.Remove(r);
                m_siren_state.Remove(r);
            }

            var removals_d = new List<int>();
            foreach (var v in d_siren_state)
            {
                if (v.Value >= 0)
                {
                    if (!API.DoesEntityExist(v.Key) || API.IsEntityDead(v.Key))
                    {
                        if (d_soundID_veh.ContainsKey(v.Key))
                        {
                            API.StopSound(d_soundID_veh[v.Key]);
                            API.ReleaseSoundId(d_soundID_veh[v.Key]);
                            removals_d.Add(v.Key);
                        }
                    }
                }
            }

            foreach (var r in removals_d)
            {
                d_soundID_veh.Remove(r);
                d_siren_state.Remove(r);
            }

        }

        private void hornCleanup()
        {
            var removals = new List<int>();
            foreach (var v in h_horn_state)
            {
                if (v.Value >= 0)
                {
                    if (!API.DoesEntityExist(v.Key) || API.IsEntityDead(v.Key))
                    {
                        if (h_soundID_veh.ContainsKey(v.Key))
                        {
                            API.StopSound(h_soundID_veh[v.Key]);
                            API.ReleaseSoundId(h_soundID_veh[v.Key]);
                            removals.Add(v.Key);
                        }
                    }
                }
            }
            foreach (var r in removals)
            {
                h_soundID_veh.Remove(r);
                h_horn_state.Remove(r);
            }
        }

        private void vehicleLightCleanup()
        {
            var removals = new List<int>();
            foreach (var v in elsVehs)
            {
                if (!API.DoesEntityExist(v.Key) || API.IsEntityDead(v.Key))
                {
                    removals.Add(v.Key);
                }
            }

            foreach (var r in removals)
            {
                elsVehs.Remove(r);
            }
        }

        private void Sound()
        {
            if (Config.playButtonPressSounds)
            {
                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
            }
        }

        private void changeLightStage(int state, int advisor, int prim, int sec)
        {
            FireServerEvent("fivelife.sync.els.change_light_stage", CitizenFX.Core.Game.Player.ServerId, state, advisor, prim, sec);
        }

        private void changeAdvisorPattern(int pat)
        {
            FireServerEvent("fivelife.sync.els.change_advisor_pattern", CitizenFX.Core.Game.Player.ServerId, pat);
        }

        private void changeSecondaryPattern(int pat)
        {
            FireServerEvent("fivelife.sync.els.change_secondary_pattern", CitizenFX.Core.Game.Player.ServerId, pat);
        }

        private void changeLightPattern(int pat)
        {
            FireServerEvent("fivelife.sync.els.change_primary_pattern", CitizenFX.Core.Game.Player.ServerId, pat);
        }

        private void OnVehicleUpdate(Dictionary<string, Shared.ELS.ElsVehicle> VehicleInfo, List<ElsState> Primaries, List<ElsState> Secondaries, List<ElsState> Advisors)
        {

            this.els_Vehicles = VehicleInfo;
            this.Primaries = Primaries;
            this.Secondaries = Secondaries;
            this.Advisors = Advisors;

            var vehId = API.GetVehiclePedIsUsing(API.GetPlayerPed(-1));
            if (els_Vehicles.ContainsKey(checkCarHash(vehId)))
            {
                API.SetVehicleSiren(vehId, false);
                FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, 0);
            }

            Tick += LightWorker;
            Tick += ExtraWorker;
            Tick += PatternStageTwoWorker;
            Tick += PatternAdvisorWorker;
            Tick += PatternStageThreeWorker;
            Tick += GuiWorker;
            Tick += Sync;
        }

        private async Task Sync()
        {
            var vehId = API.GetVehiclePedIsUsing(API.GetPlayerPed(-1));
            if (API.IsPedInVehicle(API.GetPlayerPed(-1), vehId, false))
            {
                if (elsVehs.ContainsKey(vehId))
                {
                    var veh = elsVehs[vehId];
                    changeLightStage(veh.stage, veh.advisorPattern, veh.primPattern, veh.secPattern);
                }
                if (m_siren_state.ContainsKey(vehId))
                {
                    FireServerEvent("fivelife.sync.els.set_siren_state", CitizenFX.Core.Game.Player.ServerId, m_siren_state[vehId]);
                }
                if (d_siren_state.ContainsKey(vehId))
                {
                    FireServerEvent("fivelife.sync.els.set_dual_siren_state", CitizenFX.Core.Game.Player.ServerId, d_siren_state[vehId]);
                }
                if (dualEnable.ContainsKey(vehId))
                {
                    FireServerEvent("fivelife.sync.els.set_dual_siren", CitizenFX.Core.Game.Player.ServerId, dualEnable[vehId]);
                }
            }
            await Delay(5000);
        }

        private void Draw(string text, int r, int g, int b, int alpha, float x, float y, float width, float height, int ya, bool center, int font)
        {
            API.SetTextColour(r, g, b, alpha);
            API.SetTextFont(font);
            API.SetTextScale(width, height);
            API.SetTextWrap(0.0f, 1.0f);
            API.SetTextCentre(center);
            API.SetTextDropshadow(0, 0, 0, 0, 0);
            API.SetTextEdge(1, 0, 0, 0, 205);
            API.SetTextEntry("STRING");
            API.AddTextComponentString(text);
            Function.Call((Hash)0x61BB1D9B3A95D802, ya);
            API.DrawText(x, y);
        }

        private async Task GuiWorker()
        {
            var veh = API.GetVehiclePedIsUsing(API.GetPlayerPed(-1));
            var vehN = checkCarHash(veh);

            if (!guiEnabled || !els_Vehicles.ContainsKey(vehN)) return;

            if (API.GetPedInVehicleSeat(veh, -1) == API.GetPlayerPed(-1) || API.GetPedInVehicleSeat(veh, 0) == API.GetPlayerPed(-1))
            {

                API.DrawRect(0.85f, 0.91f, 0.24f, 0.11f, 0, 0, 0, 200);

                if (getVehicleLightStage(veh) == 1)
                {
                    API.DrawRect(0.75f, 0.88f, 0.03f, 0.02f, 173, 0, 0, 225);
                    Draw("1", 0, 0, 0, 255, 0.75f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.75f, 0.88f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("1", 0, 0, 0, 255, 0.75f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }

                if (getVehicleLightStage(veh) == 2)
                {
                    API.DrawRect(0.784f, 0.88f, 0.03f, 0.02f, 173, 0, 0, 225);
                    Draw("2", 0, 0, 0, 255, 0.784f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.784f, 0.88f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("2", 0, 0, 0, 255, 0.784f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }

                if (getVehicleLightStage(veh) == 3)
                {
                    API.DrawRect(0.817f, 0.88f, 0.03f, 0.02f, 173, 0, 0, 225);
                    Draw("3", 0, 0, 0, 255, 0.817f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.817f, 0.88f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("3", 0, 0, 0, 255, 0.817f, 0.87f, 0.25f, 0.25f, 1, true, 0);
                }

                API.DrawRect(0.854f, 0.88f, 0.035f, 0.02f, 186, 186, 186, 225);
                Draw($"PRIM {lightPatternPrim}", 0, 0, 0, 255, 0.854f, 0.87f, 0.25f, 0.25f, 1, true, 0);

                API.DrawRect(0.854f, 0.91f, 0.035f, 0.02f, 186, 186, 186, 225);
                Draw($"SEC {lightPatternSec}", 0, 0, 0, 255, 0.854f, 0.9f, 0.25f, 0.25f, 1, true, 0);

                if (doesVehicleHaveTrafficAdvisor(veh))
                {
                    API.DrawRect(0.854f, 0.94f, 0.035f, 0.02f, 186, 186, 186, 225);
                    Draw($"ADV {advisorPatternSelectedIndex}", 0, 0, 0, 255, 0.854f, 0.93f, 0.25f, 0.25f, 1, true, 0);
                }

                if (h_horn_state.ContainsKey(veh) && h_horn_state[veh] == 1)
                {
                    API.DrawRect(0.75f, 0.91f, 0.03f, 0.02f, 0, 173, 0, 225);
                    Draw("HORN", 0, 0, 0, 255, 0.75f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.75f, 0.91f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("HORN", 0, 0, 0, 255, 0.75f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }

                if (dualEnable.ContainsKey(veh) && dualEnable[veh])
                {
                    API.DrawRect(0.784f, 0.91f, 0.03f, 0.02f, 0, 213, 255, 225);
                    Draw("DUAL", 0, 0, 0, 255, 0.784f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.784f, 0.91f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("DUAL", 0, 0, 0, 255, 0.784f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 11))
                {
                    API.DrawRect(0.817f, 0.91f, 0.03f, 0.02f, 255, 0, 0, 255);
                    Draw("TKD", 0, 0, 0, 255, 0.817f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }
                else
                {
                    API.DrawRect(0.817f, 0.91f, 0.03f, 0.02f, 186, 186, 186, 225);
                    Draw("TKD", 0, 0, 0, 255, 0.817f, 0.9f, 0.25f, 0.25f, 1, true, 0);
                }

                if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 1)
                {
                    if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 1)
                    {
                        API.DrawRect(0.743f, 0.94f, 0.015f, 0.02f, 0, 173, 0, 225);
                    }
                    else
                    {
                        API.DrawRect(0.75f, 0.94f, 0.03f, 0.02f, 0, 173, 0, 225);
                    }
                }
                else
                {
                    API.DrawRect(0.75f, 0.94f, 0.03f, 0.02f, 186, 186, 186, 225);
                }

                if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 1)
                {
                    if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 1)
                    {
                        API.DrawRect(0.758f, 0.94f, 0.015f, 0.02f, 0, 213, 255, 225);
                    }
                    else
                    {
                        API.DrawRect(0.75f, 0.94f, 0.03f, 0.02f, 0, 213, 255, 225);
                    }
                }

                Draw("MAIN", 0, 0, 0, 255, 0.75f, 0.93f, 0.25f, 0.25f, 3, true, 0);

                if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 2)
                {
                    if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 2)
                    {
                        API.DrawRect(0.777f, 0.94f, 0.015f, 0.02f, 0, 173, 0, 225);
                    }
                    else
                    {
                        API.DrawRect(0.784f, 0.94f, 0.03f, 0.02f, 0, 173, 0, 225);
                    }
                }
                else
                {
                    API.DrawRect(0.784f, 0.94f, 0.03f, 0.02f, 186, 186, 186, 225);
                }

                if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 2)
                {
                    if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 2)
                    {
                        API.DrawRect(0.792f, 0.94f, 0.015f, 0.02f, 0, 213, 255, 225);
                    }
                    else
                    {
                        API.DrawRect(0.784f, 0.94f, 0.03f, 0.02f, 0, 213, 255, 255);
                    }
                }

                Draw("SEC", 0, 0, 0, 255, 0.784f, 0.93f, 0.25f, 0.25f, 3, true, 0);

                if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 3)
                {
                    if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 3)
                    {
                        API.DrawRect(0.81f, 0.94f, 0.015f, 0.02f, 0, 173, 0, 225);
                    }
                    else
                    {
                        API.DrawRect(0.817f, 0.94f, 0.03f, 0.02f, 0, 173, 0, 225);
                    }
                }
                else
                {
                    API.DrawRect(0.817f, 0.94f, 0.03f, 0.02f, 186, 186, 186, 225);
                }

                if (d_siren_state.ContainsKey(veh) && d_siren_state[veh] == 3)
                {
                    if (m_siren_state.ContainsKey(veh) && m_siren_state[veh] == 3)
                    {
                        API.DrawRect(0.823f, 0.94f, 0.015f, 0.02f, 0, 213, 255, 225);
                    }
                    else
                    {
                        API.DrawRect(0.817f, 0.94f, 0.03f, 0.02f, 0, 213, 255, 255);
                    }
                }

                Draw("AUX", 0, 0, 0, 255, 0.817f, 0.93f, 0.25f, 0.25f, 3, true, 0);

                if (API.IsVehicleExtraTurnedOn(veh, 7))
                {
                    API.DrawRect(0.9f, 0.94f, 0.015f, 0.015f, els_Vehicles[vehN].extras[7].env_color.R, els_Vehicles[vehN].extras[7].env_color.G, els_Vehicles[vehN].extras[7].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.9f, 0.94f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 8))
                {
                    API.DrawRect(0.92f, 0.94f, 0.015f, 0.015f, els_Vehicles[vehN].extras[8].env_color.R, els_Vehicles[vehN].extras[8].env_color.G, els_Vehicles[vehN].extras[8].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.92f, 0.94f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 9))
                {
                    API.DrawRect(0.94f, 0.94f, 0.015f, 0.015f, els_Vehicles[vehN].extras[9].env_color.R, els_Vehicles[vehN].extras[9].env_color.G, els_Vehicles[vehN].extras[9].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.94f, 0.94f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 1))
                {
                    API.DrawRect(0.89f, 0.92f, 0.015f, 0.015f, els_Vehicles[vehN].extras[1].env_color.R, els_Vehicles[vehN].extras[1].env_color.G, els_Vehicles[vehN].extras[1].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.89f, 0.92f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 2))
                {
                    API.DrawRect(0.91f, 0.92f, 0.015f, 0.015f, els_Vehicles[vehN].extras[2].env_color.R, els_Vehicles[vehN].extras[2].env_color.G, els_Vehicles[vehN].extras[2].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.91f, 0.92f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 3))
                {
                    API.DrawRect(0.93f, 0.92f, 0.015f, 0.015f, els_Vehicles[vehN].extras[3].env_color.R, els_Vehicles[vehN].extras[3].env_color.G, els_Vehicles[vehN].extras[3].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.93f, 0.92f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 4))
                {
                    API.DrawRect(0.95f, 0.92f, 0.015f, 0.015f, els_Vehicles[vehN].extras[4].env_color.R, els_Vehicles[vehN].extras[4].env_color.G, els_Vehicles[vehN].extras[4].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.95f, 0.92f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 5))
                {
                    API.DrawRect(0.91f, 0.885f, 0.015f, 0.015f, els_Vehicles[vehN].extras[5].env_color.R, els_Vehicles[vehN].extras[5].env_color.G, els_Vehicles[vehN].extras[5].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.91f, 0.885f, 0.015f, 0.015f, 186, 186, 186, 225);
                }

                if (API.IsVehicleExtraTurnedOn(veh, 6))
                {
                    API.DrawRect(0.93f, 0.885f, 0.015f, 0.015f, els_Vehicles[vehN].extras[6].env_color.R, els_Vehicles[vehN].extras[6].env_color.G, els_Vehicles[vehN].extras[6].env_color.B, 225);
                }
                else
                {
                    API.DrawRect(0.93f, 0.885f, 0.015f, 0.015f, 186, 186, 186, 225);
                }
            }
        }

        private void OnChangeLightStage(int senderId, int stage, int advisor, int prim, int sec)
        {
            var player = new PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);

                // if canaryClient?
                API.SetVehicleAutoRepairDisabled(vehNetId, true);
                // endif

                if (!elsVehs.ContainsKey(vehNetId))
                    elsVehs[vehNetId] = new ElsVehicle();

                elsVehs[vehNetId].stage = stage;
                elsVehs[vehNetId].primPattern = prim;
                elsVehs[vehNetId].secPattern = sec;
                elsVehs[vehNetId].advisorPattern = advisor;
            }
        }

        private void OnChangePrimaryPattern(int senderId, int pat)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);

                // if canaryClient?
                API.SetVehicleAutoRepairDisabled(vehNetId, true);
                // endif

                if (!elsVehs.ContainsKey(vehNetId))
                    elsVehs[vehNetId] = new ElsVehicle();

                elsVehs[vehNetId].primPattern = pat;
            }
        }

        private void OnChangeSecondaryPattern(int senderId, int pat)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);

                // if canaryClient?
                API.SetVehicleAutoRepairDisabled(vehNetId, true);
                // endif

                if (!elsVehs.ContainsKey(vehNetId))
                    elsVehs[vehNetId] = new ElsVehicle();

                elsVehs[vehNetId].secPattern = pat;
            }
        }

        private void OnChangeAdvisorPattern(int senderId, int pat)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);

                // if canaryClient?
                API.SetVehicleAutoRepairDisabled(vehNetId, true);
                // endif

                if (!elsVehs.ContainsKey(vehNetId))
                    elsVehs[vehNetId] = new ElsVehicle();

                elsVehs[vehNetId].advisorPattern = pat;
            }
        }

        private void OnSetSirenState(int senderId, int state)
        {
            var player = new PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);
                setSirenState(vehNetId, state);
            }
        }

        private void OnSetDualSiren(int senderId, bool state)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);
                dualEnable[vehNetId] = state;
            }
        }

        private void OnSetDualSirenState(int senderId, int state)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);
                setDualSirenState(vehNetId, state);
            }
        }

        private void OnSetHornState(int senderId, int state)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var vehNetId = API.GetVehiclePedIsUsing(ped.Handle);
                setHornState(vehNetId, state);
            }
        }

        private void OnSetTakedownState(int senderId, int state)
        {
            var player = new CitizenFX.Core.PlayerList()[senderId];
            var ped = player.Character;
            if (ped.Exists() && !ped.IsDead && API.IsPedInAnyVehicle(ped.Handle, false))
            {
                var veh = API.GetVehiclePedIsUsing(ped.Handle);
                if (!elsVehs.ContainsKey(veh))
                {
                    changeLightStage(0, 1, 1, 1);
                }
                if (API.IsVehicleExtraTurnedOn(veh, 11))
                {
                    setExtraState(veh, 11, true);
                }
                else
                {
                    setExtraState(veh, 11, false);
                }
            }
        }

        private void setExtraState(int veh, int extra, bool state)
        {
            if (API.DoesEntityExist(veh) && !API.IsEntityDead(veh))
            {
                var car = checkCarHash(veh);
                if (els_Vehicles.ContainsKey(car))
                {
                    if (els_Vehicles[car].extras.ContainsKey(extra))
                    {
                        if (els_Vehicles[car].extras[extra].enabled)
                        {
                            if (API.DoesExtraExist(veh, extra))
                            {
                                API.SetVehicleExtra(veh, extra, state);
                            }
                        }
                    }
                }
            }
        }

        private void setHornState(int veh, int newstate)
        {
            if (API.DoesEntityExist(veh) && !API.IsEntityDead(veh))
            {
                if (!h_horn_state.ContainsKey(veh) || (h_horn_state.ContainsKey(veh) && newstate != h_horn_state[veh]))
                {
                    if (h_soundID_veh.ContainsKey(veh))
                    {
                        API.StopSound(h_soundID_veh[veh]);
                        API.ReleaseSoundId(h_soundID_veh[veh]);
                        h_soundID_veh.Remove(veh);
                    }

                    if (newstate == 1)
                    {
                        h_soundID_veh[veh] = API.GetSoundId();

                        if (getSirenType(veh) == 1) // fire
                        {
                            API.PlaySoundFromEntity(h_soundID_veh[veh], "VEHICLES_HORNS_FIRETRUCK_WARNING", veh, null, false, 0);
                        }
                        else
                        {
                            API.PlaySoundFromEntity(h_soundID_veh[veh], "SIRENS_AIRHORN", veh, null, false, 0);
                        }
                    }

                    h_horn_state[veh] = newstate;
                }
            }
        }

        private void setDualSirenState(int veh, int newstate)
        {
            if (API.DoesEntityExist(veh) && !API.IsEntityDead(veh))
            {
                if ((!d_siren_state.ContainsKey(veh)) || (d_siren_state.ContainsKey(veh) && newstate != d_siren_state[veh]))
                {

                    if (d_soundID_veh.ContainsKey(veh))
                    {
                        API.StopSound(d_soundID_veh[veh]);
                        API.ReleaseSoundId(d_soundID_veh[veh]);
                        d_soundID_veh.Remove(veh);
                    }

                    switch (newstate)
                    {
                        case 1:
                            d_soundID_veh[veh] = API.GetSoundId();
                            API.PlaySoundFromEntity(d_soundID_veh[veh], "VEHICLES_HORNS_SIREN_1", veh, null, false, 0);
                            break;
                        case 2:
                            d_soundID_veh[veh] = API.GetSoundId();
                            API.PlaySoundFromEntity(d_soundID_veh[veh], "VEHICLES_HORNS_SIREN_2", veh, null, false, 0);
                            break;
                        case 3:
                            d_soundID_veh[veh] = API.GetSoundId();

                            if (getSirenType(veh) == 1) // fire
                            {
                                API.PlaySoundFromEntity(d_soundID_veh[veh], "VEHICLES_HORNS_FIRETRUCK_WARNING", veh, null, false, 0);
                            }
                            else if (getSirenType(veh) == 2) // ambu
                            {
                                API.PlaySoundFromEntity(d_soundID_veh[veh], "VEHICLES_HORNS_AMBULANCE_WARNING", veh, null, false, 0);
                            }
                            else // poli
                            {
                                API.PlaySoundFromEntity(d_soundID_veh[veh], "VEHICLES_HORNS_POLICE_WARNING", veh, null, false, 0);
                            }

                            break;
                    }

                    toggleSirenMute(veh, true);
                    d_siren_state[veh] = newstate;
                }
            }
        }

        private void setSirenState(int veh, int newstate)
        {
            if (API.DoesEntityExist(veh) && !API.IsEntityDead(veh))
            {
                if ((!m_siren_state.ContainsKey(veh)) || (m_siren_state.ContainsKey(veh) && newstate != m_siren_state[veh]))
                {

                    if (m_soundID_veh.ContainsKey(veh))
                    {
                        API.StopSound(m_soundID_veh[veh]);
                        API.ReleaseSoundId(m_soundID_veh[veh]);
                        m_soundID_veh.Remove(veh);
                    }

                    switch (newstate)
                    {
                        case 1:
                            m_soundID_veh[veh] = API.GetSoundId();
                            API.PlaySoundFromEntity(m_soundID_veh[veh], "VEHICLES_HORNS_SIREN_1", veh, null, false, 0);
                            break;
                        case 2:
                            m_soundID_veh[veh] = API.GetSoundId();
                            API.PlaySoundFromEntity(m_soundID_veh[veh], "VEHICLES_HORNS_SIREN_2", veh, null, false, 0);
                            break;
                        case 3:
                            m_soundID_veh[veh] = API.GetSoundId();

                            if (getSirenType(veh) == 1) // fire
                            {
                                API.PlaySoundFromEntity(m_soundID_veh[veh], "VEHICLES_HORNS_FIRETRUCK_WARNING", veh, null, false, 0);
                            }
                            else if (getSirenType(veh) == 2) // ambu
                            {
                                API.PlaySoundFromEntity(m_soundID_veh[veh], "VEHICLES_HORNS_AMBULANCE_WARNING", veh, null, false, 0);
                            }
                            else // poli
                            {
                                API.PlaySoundFromEntity(m_soundID_veh[veh], "VEHICLES_HORNS_POLICE_WARNING", veh, null, false, 0);
                            }

                            break;
                    }

                    toggleSirenMute(veh, true);
                    m_siren_state[veh] = newstate;
                }
            }
        }

        private bool doesVehicleHaveTrafficAdvisor(int veh)
        {
            foreach (var s in Config.modelsWithTrafficAdvisor)
            {
                if (API.GetHashKey(s) == API.GetEntityModel(veh))
                {
                    return true;
                }
            }

            return false;
        }

        private bool canUseAdvisorStageThree(int veh)
        {
            foreach (var s in Config.vehicleStageThreeAdvisor)
            {
                if (API.GetHashKey(s) == API.GetEntityModel(veh))
                {
                    return true;
                }
            }

            return false;
        }

        private int getSirenType(int veh)
        {

            foreach (var s1 in Config.modelsWithFireSiren)
            {
                if (API.GetHashKey(s1) == API.GetEntityModel(veh))
                {
                    return 1;
                }
            }

            foreach (var s2 in Config.modelsWithAmbWarnSiren)
            {
                if (API.GetHashKey(s2) == API.GetEntityModel(veh))
                {
                    return 2;
                }
            }

            return 0;
        }

        private void toggleSirenMute(int veh, bool v)
        {
            if (API.DoesEntityExist(veh) && !API.IsEntityDead(veh))
            {
                API.DisableVehicleImpactExplosionActivation(veh, v);
            }
        }

        private string checkCarHash(int car)
        {
            foreach (var i in els_Vehicles)
            {
                if (API.GetEntityModel(car) == API.GetHashKey(i.Key))
                {
                    return i.Key;
                }
            }

            return "";
        }

    }
}
