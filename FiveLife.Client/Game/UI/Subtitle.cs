using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.UI
{
    public static class Subtitle
    {
        public static void Draw(string text)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_TEXT, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, 0.5f, 0.5f);
            Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0f, 1f);
            Function.Call(Hash.SET_TEXT_CENTRE, true);

            Function.Call(Hash.SET_TEXT_DROPSHADOW, 3, 255, 0, 255, 255);
            Function.Call(Hash.SET_TEXT_DROP_SHADOW);

            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_TEXT, 0.5f, 0.95f);
        }

    }
}
