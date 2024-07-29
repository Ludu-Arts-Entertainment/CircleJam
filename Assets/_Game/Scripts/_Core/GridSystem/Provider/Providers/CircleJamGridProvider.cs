using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamGridProvider : IGridProvider
{
    public IGridProvider CreateSelf()
    {
        return new CircleJamGridProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }
}
