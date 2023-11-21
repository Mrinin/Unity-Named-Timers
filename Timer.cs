using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NamedTimers
{ 
    public class Timer
    {
        public string name;
        public float time_left;
        public float duration;

        public bool loopInfinitely;
        public int loops;
        public bool runOnUnscaledTime; // runs on scaled time by default

        public Action Callback;
        public Action<float, float> OnUpdate;
    }
}