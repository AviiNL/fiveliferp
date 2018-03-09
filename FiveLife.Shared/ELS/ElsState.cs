using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.ELS
{
    public class ElsState
    {
        public int speed { get; set; }
        public Dictionary<int, Dictionary<object, int>> stages = new Dictionary<int, Dictionary<object, int>>();
    }
}
