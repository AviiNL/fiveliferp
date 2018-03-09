using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Shared.ELS
{
    public class ElsPattern
    {
        public List<ElsState> Primairies { get; set; } = new List<ElsState>();
        public List<ElsState> Secondaries { get; set; } = new List<ElsState>();
        public List<ElsState> Advisors { get; set; } = new List<ElsState>();
    }
}
