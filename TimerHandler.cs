using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NamedTimers
{
    public class TimerHandler
    {
        Dictionary<string, Timer> Timers = new();
        List<NamelessTimer> NamelessTimers = new();

        class NamelessTimer
        {
            public Action callback;
            public float time_left;
        }

        public void Tick()
        {
            ManageTimers();
            ManageNamelessTimers();
        }

        void AddTimerQuick(string name, float time, Action callback)
        {
            Timer t = new Timer();
            t.name = name;
            t.duration = time;
            t.time_left = time;

            t.Callback = callback;

            Timers.Add(name, t);
        }

        public void AddTimer(Timer t)
        {
            if (t.name == null)
            {
                Debug.LogError("Timer class passed into the AddTimer function must have a non-null name");
                return;
            }

            foreach(string s in Timers.Keys)
            {
                if(t.name == s)
                {
                    Timers.Remove(s);
                }
            }

            Timers.Add(t.name, t);
        }

        public float GetTimer(string name)
        {
            if (Timers.TryGetValue(name, out var t))
            {
                return t.time_left;
            }

            return 0;
        }

        public int SetTimer(string name, float time)
        {
            return SetTimer(name, time, null);
        }

        public int SetTimer(string name, float a, Action callback)
        {
            if (Timers.TryGetValue(name, out var timer))
            {
                timer.time_left = a;
                return 1;
            }

            AddTimerQuick(name, a, callback);

            return 2;
        }

        public void AddNamelessTimer(float a, Action callback)
        {
            NamelessTimers.Add(new NamelessTimer() { time_left = a, callback = callback });
        }

        public bool ClearTimer(string name)
        {
            if (Timers.TryGetValue(name, out var timer))
            {
                Timers.Remove(name);
                return true;
            }
            return false;
        }

        public void ClearAll()
        {
            Timers.Clear();
        }

        void ManageTimers()
        {
            List<string> keys_to_clear = new();

            foreach (var key in Timers.Keys)
            {
                Timer timer = Timers[key];

                ref float ActiveTimer = ref timer.time_left;

                if (ActiveTimer > 0)
                {
                    if (timer.runOnUnscaledTime)
                    {
                        ActiveTimer -= Time.unscaledDeltaTime;
                    }
                    else
                    {
                        ActiveTimer -= Time.deltaTime;
                    }
                }
                
                if (ActiveTimer < 0)
                {
                    ActiveTimer = 0;

                    if (timer.OnUpdate != null)
                        timer.OnUpdate(timer.time_left, timer.duration);

                    if (timer.Callback != null)
                        timer.Callback();

                    if (timer.loopInfinitely || timer.loops > 0)
                    {
                        ActiveTimer = timer.duration;
                        timer.loops--;
                    }
                    else
                    {
                        keys_to_clear.Add(key);
                    }
                }
            }

            foreach (string s in keys_to_clear)
            {
                Timers.Remove(s);
            }
        }

        void ManageNamelessTimers()
        {
            for (int i = 0; i < NamelessTimers.Count; i++)
            {
                NamelessTimer nt = NamelessTimers[i];

                nt.time_left -= Time.deltaTime;

                if (nt.time_left < 0)
                {
                    nt.callback();
                    NamelessTimers.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}