using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamMovementProvider : IMovementProvider
{
    public IMovementProvider CreateSelf()
    {
        return new CircleJamMovementProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }
}