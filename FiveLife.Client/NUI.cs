using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client
{
    public static class NUI
    {

        public enum Page
        {
            CharacterSelect,
            IDForm,
            Game
        }

        public static void Show(Page page, dynamic data = null)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                page = page.ToString(),
                data
            }));
        }

        public static void Open(Page page, dynamic data = null, bool mouse = true)
        {
            API.SetNuiFocus(true, mouse);

            var dd = JsonConvert.SerializeObject(new
            {
                page = page.ToString(),
                data
            });

            API.SendNuiMessage(dd);
        }

        public static void Close()
        {
            API.SetNuiFocus(false, false);
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                page = "Home",
                data = new { }
            }));
        }

    }
}
