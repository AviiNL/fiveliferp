using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Client
{
    public abstract class FiveLifeScript : BaseScript
    {
        private bool firstTick = true;

        public FiveLifeScript()
        {
            EventHandlers.Add("fivelife.game.initialized", new Action(OnInit));
        }

        private void OnInit()
        {
            Tick += FiveLifeScript_Tick;
        }

        public void FireEvent(string eventName, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            TriggerEvent(eventName, args);
        }

        public void FireServerEvent(string eventName, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);

            TriggerServerEvent(eventName, args);
        }

        private async Task FiveLifeScript_Tick()
        {
            if(firstTick)
            {
                Initialize();
                firstTick = false;
                return;
            }

            await Loop();
        }

        protected void RegisterEvent(string msg, Action callback)
        {
            EventHandlers.Add(msg, new Action(() => {
                callback.Invoke();
            }));
        }

        protected void RegisterEvent<T>(string msg, Action<T> callback)
        {
            EventHandlers.Add(msg, new Action<dynamic>((body) =>
            {
                if(!(body is string))
                    body = JsonConvert.SerializeObject(body);

                var response = JsonConvert.DeserializeObject<T>(body);

                callback.Invoke(response);
            }));
        }

        protected void RegisterEvent<T1, T2>(string msg, Action<T1, T2> callback)
        {
            EventHandlers.Add(msg, new Action<dynamic, dynamic>((body1, body2) =>
            {
                if (!(body1 is string))
                    body1 = JsonConvert.SerializeObject(body1);
                if (!(body2 is string))
                    body2 = JsonConvert.SerializeObject(body2);

                var response1 = JsonConvert.DeserializeObject<T1>(body1);
                var response2 = JsonConvert.DeserializeObject<T2>(body2);

                callback.Invoke(response1, response2);
            }));
        }

        protected void RegisterEvent<T1, T2, T3>(string msg, Action<T1, T2, T3> callback)
        {
            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic>((body1, body2, body3) =>
            {
                if (!(body1 is string))
                    body1 = JsonConvert.SerializeObject(body1);
                if (!(body2 is string))
                    body2 = JsonConvert.SerializeObject(body2);
                if (!(body3 is string))
                    body3 = JsonConvert.SerializeObject(body3);

                var response1 = JsonConvert.DeserializeObject<T1>(body1);
                var response2 = JsonConvert.DeserializeObject<T2>(body2);
                var response3 = JsonConvert.DeserializeObject<T3>(body3);

                callback.Invoke(response1, response2, response3);
            }));
        }

        protected void RegisterEvent<T1, T2, T3, T4>(string msg, Action<T1, T2, T3, T4> callback)
        {
            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic, dynamic>((body1, body2, body3, body4) =>
            {

                if (!(body1 is string))
                    body1 = JsonConvert.SerializeObject(body1);
                if (!(body2 is string))
                    body2 = JsonConvert.SerializeObject(body2);
                if (!(body3 is string))
                    body3 = JsonConvert.SerializeObject(body3);
                if (!(body4 is string))
                    body4 = JsonConvert.SerializeObject(body4);

                var response1 = JsonConvert.DeserializeObject<T1>(body1);
                var response2 = JsonConvert.DeserializeObject<T2>(body2);
                var response3 = JsonConvert.DeserializeObject<T3>(body3);
                var response4 = JsonConvert.DeserializeObject<T4>(body4);

                callback.Invoke(response1, response2, response3, response4);
            }));
        }

        protected void RegisterEvent<T1, T2, T3, T4, T5>(string msg, Action<T1, T2, T3, T4, T5> callback)
        {
            EventHandlers.Add(msg, new Action<dynamic, dynamic, dynamic, dynamic, dynamic>((body1, body2, body3, body4, body5) =>
            {

                if (!(body1 is string))
                    body1 = JsonConvert.SerializeObject(body1);
                if (!(body2 is string))
                    body2 = JsonConvert.SerializeObject(body2);
                if (!(body3 is string))
                    body3 = JsonConvert.SerializeObject(body3);
                if (!(body4 is string))
                    body4 = JsonConvert.SerializeObject(body4);
                if (!(body5 is string))
                    body5 = JsonConvert.SerializeObject(body5);

                var response1 = JsonConvert.DeserializeObject<T1>(body1);
                var response2 = JsonConvert.DeserializeObject<T2>(body2);
                var response3 = JsonConvert.DeserializeObject<T3>(body3);
                var response4 = JsonConvert.DeserializeObject<T4>(body4);
                var response5 = JsonConvert.DeserializeObject<T5>(body5);

                callback.Invoke(response1, response2, response3, response4, response5);
            }));
        }


        protected void RegisterNUICallback(string msg, Action callback)
        {
            API.RegisterNuiCallbackType(msg);

            EventHandlers.Add($"__cfx_nui:{msg}", new Action(() =>
            {
                callback.Invoke();
            }));
        }

        protected void RegisterNUICallback<T>(string msg, Action<T> callback)
        {
            API.RegisterNuiCallbackType(msg);

            EventHandlers.Add($"__cfx_nui:{msg}", new Action<ExpandoObject>((body) =>
            {
                var data = JsonConvert.SerializeObject(body);
                var response = JsonConvert.DeserializeObject<T>(data);

                callback.Invoke(response);
            }));
        }

        public virtual void Initialize() { }
        public virtual async Task Loop() { }
    }
}
