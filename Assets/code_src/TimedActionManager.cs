using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActionManager
{
    private Dictionary<string, TimedAction> timedActions;
    private List<string> completedTimedActions;

    public TimedActionManager()
    {
        timedActions = new Dictionary<string, TimedAction>();
        completedTimedActions = new List<string>();
    }

    public void Update(float deltaTime)
    {
        UpdateActions(deltaTime);
    }

    public bool ContainsTimedAction(string key)
    {
        return timedActions.ContainsKey(key);
    }

    public void RegisterTimedAction(string key, TimedAction timedAction)
    {
        if (timedActions.ContainsKey(key))
        {
            Debug.LogWarning(string.Format("TimedActionManager already contains TimedAction with key: {0}", key));
            return;
        }

        timedActions[key] = timedAction;
    }

    public void DeregisterTimedAction(string key)
    {
        if (!timedActions.ContainsKey(key))
        {
            Debug.LogWarning(string.Format("TimedActionManager does not contain TimedAction with key: {0}", key));
            return;
        }

        timedActions.Remove(key);
    }

    private void UpdateActions(float deltaTime)
    {
        completedTimedActions.Clear();

        foreach (KeyValuePair<string, TimedAction> kvp in timedActions)
        {
            if (kvp.Value.Complete)
            {
                completedTimedActions.Add(kvp.Key);
                continue;
            }

            kvp.Value.Update(deltaTime);
        }

        foreach (string key in completedTimedActions)
        {
            timedActions.Remove(key);
        }
    }
}
