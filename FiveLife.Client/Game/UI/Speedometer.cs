using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Game.UI
{
    public class Speedometer : FiveLifeScript
    {
        private Container rectangle = new Container(new PointF(210f, 668f), new SizeF(125, 45), Color.FromArgb(64, 0, 0, 0));
        private Text speed = new Text("", new PointF(220f, 673f), 0.40f);

        public override void Initialize()
        {
            
        }

        public override async Task Loop()
        {
            
        }
    }
}
