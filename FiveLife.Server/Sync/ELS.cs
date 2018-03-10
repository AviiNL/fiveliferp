using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Shared.ELS;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FiveLife.Server.Sync
{
    // Converted from https://github.com/MrDaGree/ELS-FiveM/blob/development/server/server.lua
    public class ELS : FiveLifeScript
    {

        private Dictionary<string, ElsVehicle> VehicleInfo = new Dictionary<string, ElsVehicle>();

        private List<ElsState> Primaries = new List<ElsState>();
        private List<ElsState> Secondaries = new List<ElsState>();
        private List<ElsState> Advisors = new List<ElsState>();

        public ELS()
        {
            if (!Directory.Exists($"{ResourceDir}\\vcf")) return;

            var vcf_files = Directory.GetFiles($"{ResourceDir}\\vcf", "*.xml", SearchOption.AllDirectories);
            var pattern_files = Directory.GetFiles($"{ResourceDir}\\patterns", "*.xml", SearchOption.AllDirectories);

            foreach (var vcf in vcf_files)
            {
                var doc = XDocument.Load(vcf);
                var name = Path.GetFileNameWithoutExtension(vcf).ToLower();
                parseObjSet(doc, name);
            }

            foreach (var pattern in pattern_files)
            {
                var doc = XDocument.Load(pattern);
                var name = Path.GetFileNameWithoutExtension(pattern).ToLower();
                parseObjSet(doc, name);

            }
        }

        private void parseObjSet(XDocument doc, string name)
        {
            if (doc.Root.Name.LocalName == "vcfroot")
            {
                parseVehData(doc, name);
            }
            else if (doc.Root.Name.LocalName == "pattern")
            {
                parsePatternData(doc, name);
            }
        }

        private void parseVehData(XDocument doc, string name)
        {
            var a = new ElsVehicle();
            a.extras = new Dictionary<int, ElsVehicleExtra>();

            foreach (var el in doc.Root.Elements())
            {
                if (el.Name.LocalName == "PRML")
                {
                    a.primType = el.Attribute("LightingFormat").Value;
                }

                if (el.Name.LocalName == "INTERFACE")
                {
                    foreach (var kid in el.Elements())
                    {
                        if (kid.Name.LocalName == "LstgActivationType")
                        {
                            if (kid.Value == "manual" || kid.Value == "auto")
                            {
                                a.activateUp = true;
                            }
                            else
                            {
                                a.activateUp = false;
                            }
                        }
                        if (kid.Name.LocalName == "InfoPanelHeaderColor")
                        {
                            if (kid.Value == "grey")
                            {
                                a.headerColor_r = 40;
                                a.headerColor_g = 40;
                                a.headerColor_b = 40;
                            }
                            if (kid.Value == "white")
                            {
                                a.headerColor_r = 255;
                                a.headerColor_g = 255;
                                a.headerColor_b = 255;
                            }
                        }
                        if (kid.Name.LocalName == "InfoPanelButtonLightColor")
                        {
                            if (kid.Value == "green")
                            {
                                a.buttonColor_r = 0;
                                a.buttonColor_g = 255;
                                a.buttonColor_b = 0;
                            }
                            if (kid.Value == "red")
                            {
                                a.buttonColor_r = 255;
                                a.buttonColor_g = 0;
                                a.buttonColor_b = 0;
                            }
                            if (kid.Value == "blue")
                            {
                                a.buttonColor_r = 0;
                                a.buttonColor_g = 0;
                                a.buttonColor_b = 255;
                            }
                            if (kid.Value == "purple")
                            {
                                a.buttonColor_r = 270;
                                a.buttonColor_g = 0;
                                a.buttonColor_b = 255;
                            }
                            if (kid.Value == "orange")
                            {
                                a.buttonColor_r = 255;
                                a.buttonColor_g = 157;
                                a.buttonColor_b = 0;
                            }
                        }
                    }
                }
                if (el.Name.LocalName == "EOVERRIDE")
                {
                    a.advisor = false;
                    foreach (var kid in el.Elements())
                    {
                        if (kid.Name.LocalName.Substring(0, kid.Name.LocalName.Length - 2).ToUpper() == "EXTRA")
                        {
                            var extra = int.Parse(kid.Name.LocalName.Substring(kid.Name.LocalName.Length - 2));
                            a.extras[extra] = new ElsVehicleExtra();
                            a.extras[extra].enabled = bool.Parse(kid.Attribute("IsElsControlled").Value);

                            if (!a.advisor)
                            {
                                if (extra == 7)
                                {
                                    if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "AMBER")
                                    {
                                        a.advisor = true;
                                    }
                                }
                            }

                            if (kid.Attribute("AllowEnvLight") != null && bool.Parse(kid.Attribute("AllowEnvLight").Value))
                            {
                                a.extras[extra].env_light = true;
                                a.extras[extra].env_pos_x = float.Parse(kid.Attribute("OffsetX").Value);
                                a.extras[extra].env_pos_y = float.Parse(kid.Attribute("OffsetY").Value);
                                a.extras[extra].env_pos_z = float.Parse(kid.Attribute("OffsetZ").Value);

                                if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "RED")
                                {
                                    a.extras[extra].env_color_r = 255;
                                    a.extras[extra].env_color_g = 0;
                                    a.extras[extra].env_color_b = 0;
                                }
                                else if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "BLUE")
                                {
                                    a.extras[extra].env_color_r = 0;
                                    a.extras[extra].env_color_g = 0;
                                    a.extras[extra].env_color_b = 255;
                                }
                                else if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "GREEN")
                                {
                                    a.extras[extra].env_color_r = 0;
                                    a.extras[extra].env_color_g = 255;
                                    a.extras[extra].env_color_b = 0;

                                }
                                else if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "AMBER")
                                {
                                    a.extras[extra].env_color_r = 255;
                                    a.extras[extra].env_color_g = 194;
                                    a.extras[extra].env_color_b = 0;
                                }
                                else if (kid.Attribute("Color") != null && kid.Attribute("Color").Value.ToUpper() == "WHITE")
                                {
                                    a.extras[extra].env_color_r = 255;
                                    a.extras[extra].env_color_g = 255;
                                    a.extras[extra].env_color_b = 255;
                                }
                            }
                        }
                    }
                }
            }

            VehicleInfo[name] = a;
        }

        private void parsePatternData(XDocument doc, string name)
        {


            foreach (var el in doc.Root.Elements())
            {
                if (el.Name.LocalName.ToUpper() == "PRIMARY")
                {
                    var Primary = new ElsState();
                    Primary.speed = int.Parse(el.Attribute("speed").Value);
                    Primary.stages = new Dictionary<int, Dictionary<object, int>>();

                    // loop el.Elements
                    foreach (var kid in el.Elements())
                    {
                        var state = int.Parse(kid.Name.LocalName.Substring(kid.Name.LocalName.Length - 2));
                        Primary.stages[state] = new Dictionary<object, int>();

                        foreach (var atrb in kid.Attributes())
                        {
                            if (atrb.Name.LocalName.Substring(0, 5).ToUpper() == "EXTRA")
                            {
                                var id = int.Parse(atrb.Name.LocalName.Substring(5));
                                Primary.stages[state][id] = bool.Parse(atrb.Value) ? 0 : 1;
                            }
                        }

                        if (kid.Attribute("Speed") != null)
                            Primary.stages[state]["speed"] = int.Parse(kid.Attribute("Speed").Value);
                    }

                    Primaries.Add(Primary);
                }
                if (el.Name.LocalName.ToUpper() == "SECONDARY")
                {
                    var Secondary = new ElsState();
                    Secondary.speed = int.Parse(el.Attribute("speed").Value);
                    Secondary.stages = new Dictionary<int, Dictionary<object, int>>();

                    // loop el.Elements
                    foreach (var kid in el.Elements())
                    {
                        var state = int.Parse(kid.Name.LocalName.Substring(kid.Name.LocalName.Length - 2));
                        Secondary.stages[state] = new Dictionary<object, int>();

                        foreach (var atrb in kid.Attributes())
                        {
                            if (atrb.Name.LocalName.Substring(0, 5).ToUpper() == "EXTRA")
                            {
                                var id = int.Parse(atrb.Name.LocalName.Substring(5));
                                Secondary.stages[state][id] = bool.Parse(atrb.Value) ? 0 : 1;
                            }
                        }

                        if (kid.Attribute("Speed") != null)
                            Secondary.stages[state]["speed"] = int.Parse(kid.Attribute("Speed").Value);
                    }

                    Secondaries.Add(Secondary);
                }
                if (el.Name.LocalName.ToUpper() == "ADVISOR")
                {
                    var Advisor = new ElsState();
                    Advisor.speed = int.Parse(el.Attribute("speed").Value);
                    Advisor.stages = new Dictionary<int, Dictionary<object, int>>();

                    // loop el.Elements
                    foreach (var kid in el.Elements())
                    {
                        var state = int.Parse(kid.Name.LocalName.Substring(kid.Name.LocalName.Length - 2));
                        Advisor.stages[state] = new Dictionary<object, int>();

                        foreach (var atrb in kid.Attributes())
                        {
                            if (atrb.Name.LocalName.Substring(0, 5).ToUpper() == "EXTRA")
                            {
                                var id = int.Parse(atrb.Name.LocalName.Substring(5));
                                Advisor.stages[state][id] = bool.Parse(atrb.Value) ? 0 : 1;
                            }
                        }

                        if (kid.Attribute("Speed") != null)
                            Advisor.stages[state]["speed"] = int.Parse(kid.Attribute("Speed").Value);
                    }

                    Advisors.Add(Advisor);
                }
            }

        }

        public override void Initialize()
        {
            RegisterEvent<Player>("fivelife.sync.els.request_vehicle_update", OnRequestVehicleUpdate);
            RegisterEvent<Player, int, string, string, string>("fivelife.sync.els.change_light_stage", OnChangeLightStage);
            RegisterEvent<Player, int, int>("fivelife.sync.els.change_part_state", OnChangePartState); // unused

            RegisterEvent<Player, string>("fivelife.sync.els.change_primary_pattern", OnChangePrimaryPattern);
            RegisterEvent<Player, string>("fivelife.sync.els.change_secondary_pattern", OnChangeSecondaryPattern);
            RegisterEvent<Player, string>("fivelife.sync.els.change_advisor_pattern", OnChangeAdvisorPattern);

            RegisterEvent<Player, bool>("fivelife.sync.els.toggle_default_siren_mute", OnToggleDefaultSirenMute); // unused
            RegisterEvent<Player, int>("fivelife.sync.els.set_siren_state", OnSetSirenState);
            RegisterEvent<Player, int>("fivelife.sync.els.set_dual_siren_state", OnSetDualSirenState);
            RegisterEvent<Player, bool>("fivelife.sync.els.set_dual_siren", OnSetDualSiren);
            RegisterEvent<Player, int>("fivelife.sync.els.set_horn_state", OnSetHornState);
            RegisterEvent<Player, int>("fivelife.sync.els.set_takedown_state", OnSetTakedownState);

        }

        private void OnSetTakedownState([FromSource] Player source, int state)
        {
            FireEvent("fivelife.sync.els.set_takedown_state", int.Parse(source.Handle), state);
        }

        private void OnSetHornState([FromSource] Player source, int state)
        {
            FireEvent("fivelife.sync.els.set_horn_state", int.Parse(source.Handle), state);
        }

        private void OnSetDualSiren([FromSource] Player source, bool state)
        {
            FireEvent("fivelife.sync.els.set_dual_siren", int.Parse(source.Handle), state);
        }

        private void OnSetDualSirenState([FromSource] Player source, int state)
        {
            FireEvent("fivelife.sync.els.set_dual_siren_state", int.Parse(source.Handle), state);
        }

        private void OnSetSirenState([FromSource] Player source, int state)
        {
            FireEvent("fivelife.sync.els.set_siren_state", int.Parse(source.Handle), state);
        }

        private void OnToggleDefaultSirenMute([FromSource] Player source, bool toggle)
        {
            FireEvent("fivelife.sync.els.toggle_default_siren_mute", int.Parse(source.Handle), toggle);
        }

        private void OnChangePrimaryPattern([FromSource] Player source, string pat)
        {
            FireEvent("fivelife.sync.els.change_primary_pattern", int.Parse(source.Handle), pat);
        }

        private void OnChangeSecondaryPattern([FromSource] Player source, string pat)
        {
            FireEvent("fivelife.sync.els.change_secondary_pattern", int.Parse(source.Handle), pat);
        }

        private void OnChangeAdvisorPattern([FromSource] Player source, string pat)
        {
            FireEvent("fivelife.sync.els.change_advisor_pattern", int.Parse(source.Handle), pat);
        }

        private void OnChangeLightStage([FromSource] Player source, int state, string advisor, string prim, string sec)
        {
            FireEvent("fivelife.sync.els.change_light_stage", int.Parse(source.Handle), state, advisor, prim, sec);
        }

        private void OnChangePartState(Player source, int part, int state)
        {
            FireEvent("fivelife.sync.els.change_part_state", int.Parse(source.Handle), part, state);
        }

        private void OnRequestVehicleUpdate([FromSource] Player source)
        {
            FireEvent(source, "fivelife.sync.els.request_vehicle_update", VehicleInfo, Primaries, Secondaries, Advisors);
        }

    }

}
