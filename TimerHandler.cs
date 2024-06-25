using System;
using System.Collections.Generic;
using UnityEngine;

namespace NamedTimers
{
    public class TimerHandler
    {
        Dictionary<string, Timer> Timers = new();
        List<NamelessTimer> NamelessTimers = new();
        Dictionary<string, Chronometer> Chronometers = new();

        class NamelessTimer
        {
            public Action callback;
            public float time_left;
        }

        class Chronometer
        {
            public float value;
            public bool isCalled;
            public bool isFinished;
        }

        public void Tick()
        {
            ManageTimers();
            ManageNamelessTimers();
            ManageChronometers();
        }

        void AddTimerQuick(string name, float time, Action callback)
        {
            Timer t = new Timer();
            t.name = name;
            t.duration = time;
            t.time_left = time;

            t.Callback = callback;
            t.preserve = true;

            Timers.Add(name, t);
        }

        public void AddTimer(Timer t)
        {
            if (t.name == null)
            {
                Debug.LogError("Timer class passed into the AddTimer function must have a non-null name");
                return;
            }

            /*foreach(string s in Timers.Keys)
            {
                if(t.name == s)
                {
                    Timers.Remove(s);
                }
            }*/

            Timers.Remove(t.name);

            if (t.time_left == 0)
                t.time_left = t.duration;

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

        public bool TriggerChronometer(string name, float target)
        {
            Chronometer c;

            if (!Chronometers.TryGetValue(name, out c))
            {
                Chronometers[name] = new Chronometer();
                c = Chronometers[name];
            }

            c.isCalled = true;

            if (c.isFinished)
                return false;

            if (c.value > target)
            {
                c.value = 0;
                c.isFinished = true;
                return true;
            }

            return false;
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

                    if (timer.OnUpdate != null)
                    { 
                        if(ActiveTimer < 0)
                            timer.OnUpdate(0, timer.duration);
                        else
                            timer.OnUpdate(timer.time_left, timer.duration);
                    }
                }

                if (ActiveTimer < 0)
                {
                    ActiveTimer = 0;

                    if (timer.Callback != null)
                        timer.Callback();

                    if (timer.loopInfinitely || timer.loops > 0)
                    {
                        ActiveTimer = timer.duration;
                        timer.loops--;
                    }
                    else
                    {
                        if (!timer.preserve)
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

        void ManageChronometers()
        {
            foreach (var kvp in Chronometers)
            {
                if (kvp.Value.isCalled)
                {
                    kvp.Value.value += Time.deltaTime;
                    kvp.Value.isCalled = false;
                }
                else
                {
                    kvp.Value.value = 0;
                    kvp.Value.isFinished = false;
                }
            }
        }
    }
}
