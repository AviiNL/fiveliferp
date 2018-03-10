using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.ELS
{
    public class Config
    {
        public static bool stagethreewithsiren = false;
        public static bool playButtonPressSounds = true;

        public static int vehicleSyncDistance = 5000;
        public static float environmentLightBrightness = 2.0f;

        public static List<string> modelsWithTrafficAdvisor = new List<string>()
        {
            "POLICE6",
            "SHERIFF"
        };

        public static List<string> modelsWithFireSiren = new List<string>()
        {
             "FIRETRUK",
        };

        public static List<string> modelsWithAmbWarnSiren = new List<string>()
        {
            "AMBULANCE",
            "FIRETRUK",
            "LGUARD",
        };

        public static List<string> vehicleStageThreeAdvisor = new List<string>()
        {
            // "FBI3"
        };

        public enum Keyboard
        {
            modifyKey = 132,
            stageChange = 85,
            guiKey = 243,
            takedown = 245,

            tone_one = 157,
            tone_two = 158,
            tone_three = 160,

            dual_toggle = 164,
            dual_one = 165,
            dual_two = 159,
            dual_three = 161,

            primary = 246,
            secondary = 303,
            advisor = 182,
        }

        public enum Controller
        {
            modifyKey = 73,
            stageChange = 80,
            takedown = 74,

            tone_one = 173,
            tone_two = 85,
            tone_three = 172,
        }

    }
}
