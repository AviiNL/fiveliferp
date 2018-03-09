using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Database
{
    public static class Counter
    {
        private static Dictionary<int, bool> used = new Dictionary<int, bool>();
        public static Dictionary<int, string> responses = new Dictionary<int, string>();
        private static int Id = 0;

        public static int GetNext()
        {
            return Id++;
        }
    }
}
