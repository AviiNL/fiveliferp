using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Sync
{
    public class Entity : FiveLifeScript
    {

        public override void Initialize()
        {
            EventHandlers.Add("fivelife.sync.entity.fadein" , new Action<int>(OnFadeIn));
            EventHandlers.Add("fivelife.sync.entity.fadeOut", new Action<int>(OnFadeOut));
        }

        private async void OnFadeOut(int obj)
        {
            TriggerClientEvent("fivelife.sync.entity.fadeout", obj);
        }

        private async void OnFadeIn(int obj)
        {
            TriggerClientEvent("fivelife.sync.entity.fadein", obj);
        }
    }
}
