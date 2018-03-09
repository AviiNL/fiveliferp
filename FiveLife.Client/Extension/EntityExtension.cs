using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenFX.Core
{
    public static class EntityExtension
    {

        public static void FadeIn(this Entity self)
        {
            Function.Call(Hash.NETWORK_FADE_IN_ENTITY, self, true, false);
            // self.Opacity = 255;
        }

        public static void FadeOut(this Entity self)
        {
            Function.Call(Hash.NETWORK_FADE_OUT_ENTITY, self, true, false);
            // self.Opacity = 0;
        }

    }
}
