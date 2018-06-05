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
        public static void Hide(this Entity self)
        {
            API.SetEntityLocallyInvisible(self.Handle);
            self.Opacity = 0;
            self.IsCollisionEnabled = false;
        }

        public static void Show(this Entity self)
        {
            API.SetEntityLocallyVisible(self.Handle);
            self.Opacity = 255;
            self.IsCollisionEnabled = true;
        }

        public static void FadeIn(this Entity self)
        {
            // self.Show(false);
            Function.Call(Hash.NETWORK_FADE_IN_ENTITY, self, true, false);
            // self.Opacity = 255;
        }

        public static void FadeOut(this Entity self)
        {
            // self.Hide(false);
            Function.Call(Hash.NETWORK_FADE_OUT_ENTITY, self, true, false);
            // self.Opacity = 0;
        }

    }
}
