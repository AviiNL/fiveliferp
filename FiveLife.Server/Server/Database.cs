using CitizenFX.Core;
using FiveLife.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server.Server
{
    class Database : BaseScript
    {

        public Database()
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");


            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Converters = new List<JsonConverter> { new MicrosecondEpochConverter() }
            };
            
            EventHandlers.Add("fivelife.database.insert", new Action<Player, int, string, string>(OnDatabaseInsert));
            EventHandlers.Add("fivelife.database.request", new Action<Player, int, string, string, List<object>>(OnDatabaseRequest));
            EventHandlers.Add("fivelife.database.update", new Action<Player, int, string, string>(OnDatabaseUpdate));
            EventHandlers.Add("fivelife.database.delete", new Action<Player, int, string, string>(OnDatabaseDelete));

            EventHandlers.Add("fivelife.database.player", new Action<Player, int>(OnDatabasePlayer));
        }

        private async void OnDatabasePlayer([FromSource] Player player, int id)
        {
            var response = Repository<Shared.Entity.Player>.FindOne(e => e.SteamId == player.Identifiers.FirstOrDefault());

            var json = JsonConvert.SerializeObject(response);

            player.TriggerEvent("fivelife.database.response", id, json);
        }

        private async void OnDatabaseInsert([FromSource] Player player, int id, string entity, string data)
        {
            Type genericType = typeof(Repository<>);
            Type entityType = Type.GetType(entity);
            Type repositoryType = genericType.MakeGenericType(Type.GetType(entity));

            dynamic obj = JsonConvert.DeserializeObject(data, entityType);

            repositoryType.InvokeMember("Insert", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { obj });

            player.TriggerEvent("fivelife.database.response", id, "true");
        }

        private void OnDatabaseUpdate([FromSource] Player player, int id, string entity, string data)
        {
            Type genericType = typeof(Repository<>);
            Type entityType = Type.GetType(entity);
            Type repositoryType = genericType.MakeGenericType(Type.GetType(entity));

            dynamic obj = JsonConvert.DeserializeObject(data, entityType);

            repositoryType.InvokeMember("Update", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { obj });

            player.TriggerEvent("fivelife.database.response", id, "true");
        }

        private async void OnDatabaseDelete([FromSource] Player player, int id, string entity, string data)
        {
            Type genericType = typeof(Repository<>);
            Type entityType = Type.GetType(entity);
            Type repositoryType = genericType.MakeGenericType(Type.GetType(entity));

            dynamic obj = JsonConvert.DeserializeObject(data, entityType);

            repositoryType.InvokeMember("Delete", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { obj.Id });

            player.TriggerEvent("fivelife.database.response", id, "true");
        }

        private async void OnDatabaseRequest([FromSource] Player player, int id, string entity, string method, List<object> arguments)
        {
            Console.WriteLine($"Processing {id}");
            Type genericType = typeof(Repository<>);
            Type entityType = Type.GetType(entity);

            Type repositoryType = genericType.MakeGenericType(Type.GetType(entity));
            
            var response = repositoryType.InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, arguments.ToArray());
            var json = JsonConvert.SerializeObject(response);

            Console.WriteLine($"Sending response for {id}");
            player.TriggerEvent("fivelife.database.response", id, json);
        }
    }
}
