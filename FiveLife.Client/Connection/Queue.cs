using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Connection
{
    public class Queue : BaseScript
    {
        public Queue()
        {
            Tick += Queue_Tick;
        }

        private async Task Queue_Tick()
        {
            if(API.NetworkIsSessionStarted())
            {
                TriggerServerEvent("fivelife.queue.accepted");
                Tick -= Queue_Tick;
            }
        }
    }
}
