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

        public int headerColor_r { get; set; }
        public int headerColor_g { get; set; }
        public int headerColor_b { get; set; }

        public int buttonColor_r { get; set; }
        public int buttonColor_g { get; set; }
        public int buttonColor_b { get; set; }

        public bool advisor { get; set; }
        public Dictionary<int, ElsVehicleExtra> extras { get; set; }
    }
}
