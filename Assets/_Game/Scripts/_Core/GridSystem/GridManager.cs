using System;
using UnityEngine;

public class GridManager : IManager
{
    private IGridProvider _gridProvider;
    public IManager CreateSelf()
    {
        return new GridManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _gridProvider = GridProviderFactory.Create(gameInstaller.Customizer.GridProvider);
        _gridProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _gridProvider != null;
    }

    public void CreateGrid(int circleCount, Transform parent)
    {
        _gridProvider.CreateGrid(circleCount, parent);
    }

    public void StartRotateCircle(int circleIdx)
    {
        _gridProvider.StartRotateCircle(circleIdx);
    }
    
    public void RotateCircle(int circleIdx, float angle)
    {
        _gridProvider.RotateCircle(circleIdx, angle);
    }

    public void StopRotateCircle(int circleIdx)
    {
        _gridProvider.StopRotateCircle(circleIdx);
    }

    public void ResetGrid()
    {
        _gridProvider.ResetGrid();
    }
}