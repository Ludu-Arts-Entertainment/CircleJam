using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamGridProvider : IGridProvider
{
    private const int ONE_CIRCLE_GRID_COUNT = 12;

    private Dictionary<int, List<Transform>> _circleGrids = new ();
    public IGridProvider CreateSelf()
    {
        return new CircleJamGridProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    public void CreateGrid(int circleCount, Transform parent)
    {
        /* TODO: Reset Grid Function*/
        _circleGrids.Clear();   
        
        var exitDoor = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>("ExitDoor", parent: parent);
        exitDoor.transform.localPosition = Vector3.zero;
        exitDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < circleCount; i++)
        {
            var gridParent = new GameObject("CircleGrid_" + i);
            gridParent.transform.SetParent(parent);
            gridParent.transform.localPosition = Vector3.zero;

            _circleGrids.Add(i, new List<Transform>());

            for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
            {
                var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"GridLevel_{i+1}", parent: gridParent.transform);
                grid.transform.localPosition = Vector3.zero;
                grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                _circleGrids[i].Add(grid.transform);
            }
        }
    }
}
