using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TimedAction
{
    private float triggerTime;
    private Action action;

    public bool Complete
    {
        get { return triggerTime <= 0f; }
    }

    public TimedAction(Action action, float triggerTime)
    {
        this.action = action;
        this.triggerTime = triggerTime;
    }

    public void Update(float deltaTime)
    {
        triggerTime -= deltaTime;

        if (Complete)
        {
            TriggerAction();
        }
    }

    private void TriggerAction()
    {
        action();
    }
}
