# Unity-Named-Timers
Automatic timers for Unity that have names (IDs) so you can check their timers while they are running, very useful for cooldowns

To get started, add `NamedTimers` to namespaces. Then create a new timer handler and call its `Tick` function in every Update call.:


    using NamedTimers;
    
    ...
    
    TimerHandler th;
    void Start()
    {
        th = new TimerHandler();
    }

    void Update()
    {
        th.Tick();
    }
    
There are 3 ways to create new timers, SetTimer, AddTimer, and AddNamelessTimer.
There are also 3 other methods, GetTimer, ClearTimer, and ClearAll.

SetTimer: Creates a new named timer, and optionally takes a callback. If a timer with that name exists, this overrides it.

AddTimer: Creates a new named timer with more parameters. If a timer with that name exists, this overrides it.

AddNamelessTimer: Creates a new timer that has no name, meaning if ran multiple times, it will create a new timer each timer.

GetTimer: Get the remaining time of a timer by passing its name

ClearTimer: Remove that timer (does not call the callback)

ClearAll: Clears all timers associated with this TimerHandler

Code Example:

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using NamedTimers;
    
    public class Test : MonoBehaviour
    {
        TimerHandler th;
    
        void Start()
        {
            th = new TimerHandler();
    
            // Call ILoveYou every 2 seconds, starting this very frame
            th.AddTimer(new Timer()
            {
                name = "Call ILoveYou immediately, than call it again every 2 seconds",
                time_left = 0,
                duration = 2,
                loopInfinitely = true,
                Callback = () => { ILoveYou(); },
            });
        }
    
        void Update()
        {
            th.Tick();
    
            if(Input.GetKeyDown(KeyCode.A))
            {
                // If a timer is not yet set, it returns 0, so this code will work the first time it is ran
                // If a timer is done, then it will also always return 0
                // SetTimer can also optionally take a callback function at the end, but this can be omitted for cooldowns such as these
                // This is usually how you want to use Unity Timers
                if (th.GetTimer("PrintCooldown") == 0)
                {
                    print("Pressing A now has a cooldown of 2 seconds");
                    th.SetTimer("PrintCooldown", 2);
                }
            }
    
            if (Input.GetKeyDown(KeyCode.S))
            {
                // If you press, S, it wall call ILoveYou in 2 seconds. But because it will always try to call with the same name,
                // Pressing S again before the 2 seconds are up will reset the timer. Sometimes, this is the intended behaviour.
                th.SetTimer("PrintCooldown", 2, () => print("Please do not spam S :( "));
            }
    
            if (Input.GetKeyDown(KeyCode.D))
            {
                // Nameless timers, will always call ILoveYou after 3 seconds. Because the timer has no name, the button can be spammed
                th.AddNamelessTimer(3, () => print("I am unnamed, and thus prone to being spammed"));
            }
    
            if (Input.GetKeyDown(KeyCode.F))
            {
                // AddTimer, adds a timer with more parameters to 
                var t = new Timer()
                {
                    name = "This is a timer class",
                    duration = 1, // Ran every second
                    loops = 2, // Ran twice
                    loopInfinitely = false, // If true, will ignore the loops variable
                    runOnUnscaledTime = true, // Timer will still update every Update, but it will count real life time
                    Callback = () => ILoveYou(), // Callback for when the function is done
    
                    // Ran every frame the timer updates, passing the time remaining and the initial duration
                    OnUpdate = (time_left, duration) => ILoveYouAtThisTime(time_left, duration),
                };
                th.AddTimer(t);
            }
        }
    
        void ILoveYouAtThisTime(float time_left, float duration)
        {
            print($"We are {(1 - (time_left / duration)) * 100}% of the way there!");
        }
    
        void ILoveYou()
        {
            print("<3");
        }
    }

