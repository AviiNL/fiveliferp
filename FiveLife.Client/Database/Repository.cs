using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client.Database
{
    public static class Repository<T> where T : Shared.Entity.IEntity
    {
        private static EventHandlerDictionary EventHandlers;

        public static void Init(EventHandlerDictionary events)
        {
            if (EventHandlers != null) return;
            EventHandlers = events;

            EventHandlers.Add("fivelife.database.response", new Action<int, string>(OnDataReceived));
        }

        private static void OnDataReceived(int arg1, string arg2)
        {
            Counter.responses.Add(arg1, arg2);
        }

        public static async Task<IEnumerable<T>> GetAll(bool refresh = true)
        {
            return await Send<IEnumerable<T>>("GetAll", refresh);
        }

        public static async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, bool refresh = false)
        {
            var sanitized = (Expression<Func<T, bool>>)new Literalizer().Visit(predicate);

            var str = sanitized.ToString();
            var variable = str.Substring(0, str.IndexOf("=>"));
            str = str.Substring(str.IndexOf("=>") + 2).Replace(variable.Trim() + ".", "").Replace("=>", "").Trim();
            str = str.Replace("AndAlso", "&&").Replace("OrElse", "||").Replace("Convert(e, IEntity).", "").Trim('(', ')');
            return await Send<IEnumerable<T>>("Find", str, refresh);
        }

        public static async Task<T> FindOne(Expression<Func<T, bool>> predicate, bool refresh = false)
        {
            return (await Find(predicate, refresh)).FirstOrDefault();
        }

        public static async Task<T> GetById(int Id, bool refresh = false)
        {
            return await FindOne(e => e.Id == Id, refresh);
        }

        public static async Task<T> GetBySource()
        {
            var id = Counter.GetNext();

            BaseScript.TriggerServerEvent("fivelife.database.player", id);

            while (!Counter.responses.ContainsKey(id))
            {
                await BaseScript.Delay(1);
            }

            var response = JsonConvert.DeserializeObject<T>(Counter.responses[id]);
            Counter.responses.Remove(id);
            return response;
        }

        public static async Task Insert(T obj)
        {
            var type = typeof(T).AssemblyQualifiedName;
            var id = Counter.GetNext();

            BaseScript.TriggerServerEvent("fivelife.database.insert", id, type, JsonConvert.SerializeObject(obj));

            while (!Counter.responses.ContainsKey(id))
            {
                await BaseScript.Delay(1);
            }

            Counter.responses.Remove(id);
        }

        public static async Task Update(T obj)
        {
            var type = typeof(T).AssemblyQualifiedName;
            var id = Counter.GetNext();

            BaseScript.TriggerServerEvent("fivelife.database.update", id, type, JsonConvert.SerializeObject(obj));

            while (!Counter.responses.ContainsKey(id))
            {
                await BaseScript.Delay(1);
            }

            Counter.responses.Remove(id);
        }

        public static async void Delete(T obj)
        {
            var type = typeof(T).AssemblyQualifiedName;

            BaseScript.TriggerServerEvent("fivelife.database.delete", type, JsonConvert.SerializeObject(obj));
        }

        public static async void Delete(int Id)
        {
            var type = typeof(T).AssemblyQualifiedName;

            BaseScript.TriggerServerEvent("fivelife.database.delete", type, Id);
        }

        private static async Task<T1> Send<T1>(string method, params object[] args)
        {
            var id = Counter.GetNext();

            var type = typeof(T).AssemblyQualifiedName;

            BaseScript.TriggerServerEvent("fivelife.database.request", id, type, method, args);

            while (!Counter.responses.ContainsKey(id))
            {
                await BaseScript.Delay(1);
            }

            var response = JsonConvert.DeserializeObject<T1>(Counter.responses[id]);
            Counter.responses.Remove(id);
            return response;
        }
    }
}
