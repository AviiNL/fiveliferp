using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Server
{
    public abstract class FiveLifeScript : BaseScript
    {
        private bool firstTick = true;
        protected string ResourceDir = "";
        public FiveLifeScript()
        {
            ResourceDir = Directory.GetDirectories(Directory.GetCurrentDirectory(), $"resources\\*{API.GetCurrentResourceName()}", SearchOption.AllDirectories).FirstOrDefault();

            EventHandlers.Add("fivelife.database.connected", new Action(OnInit));
        }

        private void OnInit()
        {
            Tick += FiveLifeScript_Tick;
        }

        private async Task FiveLifeScript_Tick()
        {
            if (firstTick)
            {
                firstTick = false;
                Initialize();
                return;
            }

            await Loop();
        }

        protected void RegisterEvent(string msg, Action callback)
        {
            EventHandlers.Add(msg, new Action(() =>
            {
                callback.Invoke();
            }));
        }

        protected void RegisterEvent<T>(string msg, Action<T> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic>((body1) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                       new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                       JsonConvert.DeserializeObject<T>(body1);

                callback.Invoke(arg1);

            }));
        }

        protected void RegisterEvent<T1, T2>(string msg, Action<T1, T2> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic, dynamic>((body1, body2) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                        new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                        JsonConvert.DeserializeObject<T1>(body1);
                
                dynamic arg2 = sources.Contains(1) ?
                        new PlayerList()[body2.GetType() == typeof(string) ? Int32.Parse(body2) : body2] :
                        JsonConvert.DeserializeObject<T2>(body2);

                callback.Invoke(arg1, arg2);
            }));
        }

        protected void RegisterEvent<T1, T2, T3>(string msg, Action<T1, T2, T3> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic>((body1, body2, body3) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                        new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                        JsonConvert.DeserializeObject<T1>(body1);

                dynamic arg2 = sources.Contains(1) ?
                        new PlayerList()[body2.GetType() == typeof(string) ? Int32.Parse(body2) : body2] :
                        JsonConvert.DeserializeObject<T2>(body2);

                dynamic arg3 = sources.Contains(2) ?
                        new PlayerList()[body3.GetType() == typeof(string) ? Int32.Parse(body3) : body3] :
                        JsonConvert.DeserializeObject<T3>(body3);

                callback.Invoke(arg1, arg2, arg3);
            }));
        }

        protected void RegisterEvent<T1, T2, T3, T4>(string msg, Action<T1, T2, T3, T4> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic, dynamic>((body1, body2, body3, body4) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                        new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                        JsonConvert.DeserializeObject<T1>(body1);

                dynamic arg2 = sources.Contains(1) ?
                        new PlayerList()[body2.GetType() == typeof(string) ? Int32.Parse(body2) : body2] :
                        JsonConvert.DeserializeObject<T2>(body2);

                dynamic arg3 = sources.Contains(2) ?
                        new PlayerList()[body3.GetType() == typeof(string) ? Int32.Parse(body3) : body3] :
                        JsonConvert.DeserializeObject<T3>(body3);

                dynamic arg4 = sources.Contains(3) ?
                        new PlayerList()[body4.GetType() == typeof(string) ? Int32.Parse(body4) : body4] :
                        JsonConvert.DeserializeObject<T4>(body4);

                callback.Invoke(arg1, arg2, arg3, arg4);
            }));
        }

        protected void RegisterEvent<T1, T2, T3, T4, T5>(string msg, Action<T1, T2, T3, T4, T5> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic, dynamic, dynamic>((body1, body2, body3, body4, body5) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                        new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                        JsonConvert.DeserializeObject<T1>(body1);

                dynamic arg2 = sources.Contains(1) ?
                        new PlayerList()[body2.GetType() == typeof(string) ? Int32.Parse(body2) : body2] :
                        JsonConvert.DeserializeObject<T2>(body2);

                dynamic arg3 = sources.Contains(2) ?
                        new PlayerList()[body3.GetType() == typeof(string) ? Int32.Parse(body3) : body3] :
                        JsonConvert.DeserializeObject<T3>(body3);

                dynamic arg4 = sources.Contains(3) ?
                        new PlayerList()[body4.GetType() == typeof(string) ? Int32.Parse(body4) : body4] :
                        JsonConvert.DeserializeObject<T4>(body4);

                dynamic arg5 = sources.Contains(4) ?
                        new PlayerList()[body5.GetType() == typeof(string) ? Int32.Parse(body5) : body5] :
                        JsonConvert.DeserializeObject<T5>(body5);

                callback.Invoke(arg1, arg2, arg3, arg4, arg5);
            }));
        }

        protected void RegisterEvent<T1, T2, T3, T4, T5, T6>(string msg, Action<T1, T2, T3, T4, T5, T6> callback)
        {
            List<int> sources = new List<int>();
            foreach (var param in callback.Method.GetParameters())
                foreach (var attr in param.CustomAttributes)
                    if (param.ParameterType == typeof(Player) && attr.AttributeType == typeof(FromSourceAttribute))
                        sources.Add(callback.Method.GetParameters().ToList().IndexOf(param));

            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((body1, body2, body3, body4, body5, body6) =>
            {
                Console.WriteLine(msg);

                dynamic arg1 = sources.Contains(0) ?
                        new PlayerList()[body1.GetType() == typeof(string) ? Int32.Parse(body1) : body1] :
                        JsonConvert.DeserializeObject<T1>(body1);

                dynamic arg2 = sources.Contains(1) ?
                        new PlayerList()[body2.GetType() == typeof(string) ? Int32.Parse(body2) : body2] :
                        JsonConvert.DeserializeObject<T2>(body2);

                dynamic arg3 = sources.Contains(2) ?
                        new PlayerList()[body3.GetType() == typeof(string) ? Int32.Parse(body3) : body3] :
                        JsonConvert.DeserializeObject<T3>(body3);

                dynamic arg4 = sources.Contains(3) ?
                        new PlayerList()[body4.GetType() == typeof(string) ? Int32.Parse(body4) : body4] :
                        JsonConvert.DeserializeObject<T4>(body4);

                dynamic arg5 = sources.Contains(4) ?
                        new PlayerList()[body5.GetType() == typeof(string) ? Int32.Parse(body5) : body5] :
                        JsonConvert.DeserializeObject<T5>(body5);

                dynamic arg6 = sources.Contains(5) ?
                        new PlayerList()[body6.GetType() == typeof(string) ? Int32.Parse(body6) : body6] :
                        JsonConvert.DeserializeObject<T6>(body6);

                callback.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }));
        }

        public void FireEvent(string eventName, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            TriggerClientEvent(eventName, args);
        }

        public void FireEvent(int[] players, string eventName, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            foreach (var id in players)
            {
                new PlayerList()[id].TriggerEvent(eventName, args);
            }
        }

        public void FireEvent(Player player, string eventName, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            player.TriggerEvent(eventName, args);
        }

        public virtual void Initialize() { }
        public virtual async Task Loop() { }
    }
}
