using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.ELS
{
    public class ElsVehicle
    {
        public string primType { get; set; }
        public bool activateUp { get; set; }
        public System.Drawing.Color headerColor { get; set; }
        public System.Drawing.Color buttonColor { get; set; }
        public bool advisor { get; set; }
        public Dictionary<int, ElsVehicleExtra> extras { get; set; }
    }
}
