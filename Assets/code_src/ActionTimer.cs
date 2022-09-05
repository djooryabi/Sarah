using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTimer
{
    private List<Action> actions;
    public int Id { get; private set; }
    public float TimeLeft { get; private set;}

    public ActionTimer(int id, List<Action> actions, float timeLeft)
    {
        Id = id;
        TimeLeft = timeLeft;
        this.actions = actions;
    }

    public bool Completed()
    {
        return TimeLeft <= 0f;
    }

    public void DecrementTimer(float deltaTime)
    {
        TimeLeft -= deltaTime;

        if (Completed())
        {
            TriggerActions();
            actions.Clear();
        }
    }

    private void TriggerActions()
    {
        foreach (Action action in actions)
        {
            action.Invoke();
        }
    }
}
