using CitizenFX.Core;
using CitizenFX.Core.Native;
using FiveLife.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Connection
{
    public class Whitelist : BaseScript
    {

        Queue queue = new Queue();
        Dictionary<string, Player> Playing = new Dictionary<string, Player>();

        private int MAX_PLAYERS = 30; // 30??

        private bool blocked = true;

        public Whitelist()
        {
            Console.WriteLine("Loading Whitelist");

            API.SetConvar("sv_maxclients", MAX_PLAYERS.ToString());

            EventHandlers.Add("fivelife.database.connected", new Action(OnConnected));
        }

        private async void OnConnected()
        {
            EventHandlers.Add("playerConnecting", new Action<Player, string, CallbackDelegate, dynamic>(OnPlayerConnect));
            EventHandlers.Add("playerDropped", new Action<Player, string>(OnPlayerDropped));

            EventHandlers["fivelife.queue.accepted"] += new Action<Player>(OnAccepted);

            blocked = false;
        }

        private async void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Console.WriteLine($"{player.Name} disconnected: {reason}");
            queue.Remove(player);
        }

        private async void OnPlayerConnect([FromSource] Player player, string playerName, CallbackDelegate kickCallback, dynamic deferral)
        {
            deferral.defer();

            if (blocked)
            {
                deferral.done("Server is still booting, please try again later");
                await Delay(5000);
                return;
            }


            await Delay(100);
            deferral.update("Connecting...");

            var id = player.Identifiers.FirstOrDefault();
            var name = player.Name;

            #region - Whitelist -

            if (!id.StartsWith("steam:"))
            {
                TriggerEvent("fivelife.whitelist.denied");

                Console.WriteLine($"Access Denied {name} [{id}] not using steamId");

                deferral.update("Unable to connect without steam");
                await Delay(5000);
                deferral.done("Unable to connect without steam");

                return;
            }

            var dbPlayer = Database.SqLite.Repository<Shared.Entity.Player>.FindOne(e => e.SteamId == id, true);
            if (dbPlayer == null || dbPlayer.Rank == 0)
            {
                TriggerEvent("fivelife.whitelist.denied");

                Console.WriteLine($"Access Denied {name} [{id}] not whitelisted");

                deferral.update("You are not whitelisted");
                await Delay(5000);
                deferral.done("You are not whitelisted");
                
                return;
            }

            #endregion

            #region - Queue - 

            var ConnectTime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            if (queue.Contains(id))
            {
                queue.Remove(id);
                Console.WriteLine($"{name} [{id}] was remove from the queue, they cancelled connecting and attempted to rejoin");
            }
            
            queue.Add(player, dbPlayer.Priority);
            var qm = queue.Get(id);

            Console.WriteLine($"Added {name} [{id}] to the queue in position {qm.Position}");
            
            var dots = "";
            while (true)
            {
                var playerCount = new PlayerList().Count();

                qm = queue.Get(id);
                if (qm == null) return;
                
                dots += ".";
                if(dots.Length == 4)
                {
                    dots = "";
                }
                if (((queue.Count() - qm.Position) + playerCount) < (MAX_PLAYERS))
                {
                    break;
                }

                deferral.update($"You are in queue position {qm.Position} out of {queue.Count()}. Please wait{dots}");
                
                await Delay(1000);
            }
            
            deferral.done();
            #endregion

        }

        private void OnAccepted([FromSource] Player player)
        {
            Console.WriteLine($"Removed {player.Name} [{player.Identifiers.FirstOrDefault()}] from queue, added to server ({new PlayerList().Count()})");
            queue.Remove(player);
        }

    }
}
