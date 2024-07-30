using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamGridProvider : IGridProvider
{
    private const int ONE_CIRCLE_GRID_COUNT = 12;

    private Dictionary<Transform, List<GridNode>> _circleGridsByParent = new ();
    private Dictionary<int, Transform> _circleGridsParentById = new ();
    public IGridProvider CreateSelf()
    {
        return new CircleJamGridProvider();
    }

    public void Initialize(Action onReady)
    {
        onReady?.Invoke();
    }

    private GridParent gridParentObject;
    public void CreateGrid(int circleCount, Transform parent)
    {
        if(gridParentObject == null)
        {
            gridParentObject = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridParent>("GridParent", parent: parent);
        }

        for (int i = 0; i < circleCount; i++)
        {
            var gridParent = gridParentObject.GridCircleParents[i];
            _circleGridsByParent.Add(gridParent.transform, new List<GridNode>());
            _circleGridsParentById.Add(i, gridParent.transform);

            for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
            {
                var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}", parent: gridParent.transform);
                grid.Initialize(i);
                grid.transform.localPosition = Vector3.zero;
                grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                _circleGridsByParent[gridParent.transform].Add(grid);
            }
        }
    }

    public void RotateCircle(int circleIdx, float angle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;

        _circleGridsParentById[circleIdx].Rotate(Vector3.up, angle, Space.World);
    }

    public void StopRotateCircle(int circleIdx)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;
        var angle = (360 / ONE_CIRCLE_GRID_COUNT);
        _circleGridsParentById[circleIdx].rotation = Quaternion.Euler(0, Mathf.Round(_circleGridsParentById[circleIdx].eulerAngles.y / angle) * angle, 0);
    }

    public void ResetGrid()
    {
        int i = 0;
        foreach (var circleGrid in _circleGridsByParent)
        {
            foreach (var grid in circleGrid.Value)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"GridLevel_{i+1}", grid);
            }
            i++;
        }

        _circleGridsByParent.Clear();   
        _circleGridsParentById.Clear();
    }
}
