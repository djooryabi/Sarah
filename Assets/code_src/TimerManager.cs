using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager
{
    private Dictionary<int, ActionTimer> timers;
    private List<ActionTimer> completedTimers;

    public TimerManager()
    {
        timers = new Dictionary<int, ActionTimer>();
        completedTimers = new List<ActionTimer>();
    }

    public bool ContainsTimer(int timerId)
    {
        return timers.ContainsKey(timerId);
    }

    public void Update(float deltaTime)
    {
        completedTimers.Clear();

        foreach (KeyValuePair<int, ActionTimer> timer in timers)
        {
            timer.Value.DecrementTimer(deltaTime);
            
            if (timer.Value.Completed())
            {
                completedTimers.Add(timer.Value);
            }
        }

        foreach (ActionTimer timer in completedTimers)
        {
            timers.Remove(timer.Id);
        }
    }

    public void RegisterTimer(ActionTimer timer)
    {
        if (timers.ContainsKey(timer.Id))
        {
            Debug.LogError(string.Format("Timer {0} already registered.", timer.Id));
            return;
        }

        timers[timer.Id] = timer;
    }

    public void DeregisterTimer(ActionTimer timer)
    {
        if (timers.ContainsKey(timer.Id))
        {
            timers.Remove(timer.Id);
        }
    }
}
