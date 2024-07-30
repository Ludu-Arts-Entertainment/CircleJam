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