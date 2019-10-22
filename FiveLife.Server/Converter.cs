using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server
{
    public static class Converter
    {

        public static long ConvertSteamID64(string id)
        {
            var hex = id.Substring(id.IndexOf(':') + 1);
            return Int64.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

    }
}
