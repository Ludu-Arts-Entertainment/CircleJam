using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamGoalProvider : IGoalProvider
{
    public IGoalProvider CreateSelf()
    {
        return new CircleJamGoalProvider();
    }

    public void Initialize(System.Action onReady)
    {
        onReady?.Invoke();
    }
}

public enum GoalColors
{
    None = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
    Yellow = 4,
    Pink = 5,
}