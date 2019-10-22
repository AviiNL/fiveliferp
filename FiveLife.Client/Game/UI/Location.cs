using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.UI
{
    public class Location : FiveLifeScript
    {
        private Container rectangle = new Container(new PointF(210f, 668f), new SizeF(125, 45), Color.FromArgb(64, 0, 0, 0));

        private Text zoneName = new Text("", new PointF(220f, 673f), 0.40f);
        private Text streetName = new Text("", new PointF(220f, 690f), 0.30f);

        public override void Initialize()
        {
            streetName.Shadow = true;
            streetName.Outline = true;

            zoneName.Font = Font.HouseScript;
            zoneName.Shadow = true;
            zoneName.Outline = true;

            rectangle.Items.Add(streetName);
            rectangle.Items.Add(zoneName);
        }

        public override async Task Loop()
        {
            if (CitizenFX.Core.Game.Player.Character.CurrentVehicle == null) return;

            Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, HudComponent.AreaName);
            Function.Call(Hash.HIDE_HUD_COMPONENT_THIS_FRAME, HudComponent.StreetName);

            streetName.Caption = String.Format("{0} on {1}", HeadingToText(CitizenFX.Core.Game.Player.Character.Heading), CitizenFX.Core.World.GetStreetName(CitizenFX.Core.Game.Player.Character.Position));
            zoneName.Caption = CitizenFX.Core.World.GetZoneLocalizedName(CitizenFX.Core.Game.Player.Character.Position);

            var width = (zoneName.Width > streetName.Width ? zoneName.Width : streetName.Width) + 20;
            rectangle.Size = new SizeF(width, 45);
            rectangle.Position = new PointF(1280f - width, 210f);
            zoneName.Position = new PointF(10, 5);
            streetName.Position = new PointF(10, 22);

            rectangle.ScaledDraw();
        }

        private string HeadingToText(float heading)
        {
            var val = (int)((heading / 22.5f) + 0.5f);
            var arr = new string[] { "N", "NNW", "NW", "WNW", "W", "WSW", "SW", "SSW", "S", "SSE", "SE", "ESE", "E", "ENE", "NE", "NNE" };
            return arr[(val % 16)];
        }
    }
}
