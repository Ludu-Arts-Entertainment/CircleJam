using System;
using UnityEngine;

public interface IGridProvider 
{
    IGridProvider CreateSelf();
    void Initialize(Action onReady);
    void CreateGrid(int circleCount, Transform parent);
}