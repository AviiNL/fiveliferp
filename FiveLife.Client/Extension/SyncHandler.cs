using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Extension
{
    class SyncHandler : FiveLifeScript
    {
        public override void Initialize()
        {
            EventHandlers.Add("fivelife.sync.entity.fadein", new Action<int>(OnFadeIn));
            EventHandlers.Add("fivelife.sync.entity.fadeOut", new Action<int>(OnFadeOut));
        }

        private void OnFadeOut(int obj)
        {
            Debug.WriteLine("{0}", $"FadeOut: {obj}");
            var e = Function.Call<Entity>(Hash.NET_TO_ENT, obj);
            Function.Call(Hash.NETWORK_FADE_OUT_ENTITY, e, true, false);
        }

        private void OnFadeIn(int obj)
        {
            Debug.WriteLine("{0}", $"FadeIn: {obj}");
            var e = Function.Call<Entity>(Hash.NET_TO_ENT, obj);
            Function.Call(Hash.NETWORK_FADE_IN_ENTITY, e, false, true);
        }
    }
}
