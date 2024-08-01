using System;
using UnityEngine;

public interface IGridProvider 
{
    IGridProvider CreateSelf();
    void Initialize(Action onReady);
    void CreateGrid(int circleCount, Transform parent);
    void StartRotateCircle(int circleIdx);
    void RotateCircle(int circleIdx, float angle);
    void StopRotateCircle(int circleIdx, float totalAngle);
    bool CheckAnyObstacle(int circleIdx, int gridIdx);
    void ResetGrid();
}