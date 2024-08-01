using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleJamGridProvider : IGridProvider
{
    private const int ONE_CIRCLE_GRID_COUNT = 12;
    private Dictionary<int, List<int>> _circleIdxs = new ();
    private Dictionary<int, CircleData> _circleGridsParentById = new ();
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
            var circleData = new CircleData();
            circleData.CircleTransform = gridParentObject.GridCircleParents[i];
            circleData.GridNodes = new List<GridNode>();

            _circleIdxs.Add(i, new List<int>());

            if(i == 1)
            {
                circleData.IsCircleWater = true;
                var waterObject = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"WaterLevel_{i+1}", parent: circleData.CircleTransform);
                waterObject.localPosition = Vector3.zero;

                for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
                {
                    var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}", parent: circleData.CircleTransform);
                    var gridNodeData = new GridNodeData();
                    
                    if(j == 8)
                    {
                        gridNodeData.GridType = GridType.FixedPath;
                        gridNodeData.FixedPathType = FixedPathType.Bridge;
                    }
                    else
                    {
                        gridNodeData.GridType = GridType.Empty;
                    }
                    gridNodeData.CircleLevel = i;
                    gridNodeData.GridIdx = j;
                    grid.Initialize(gridNodeData);
                    grid.transform.localPosition = Vector3.zero;
                    grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                    circleData.GridNodes.Add(grid);
                    _circleIdxs[i].Add(j);
                }
            }
            else
            {
                circleData.IsCircleWater = false;

                for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
                {
                    var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}", parent: circleData.CircleTransform);
                    var gridNodeData = new GridNodeData();
                    gridNodeData.GridType = GridType.Normal;
                    gridNodeData.CircleLevel = i;
                    gridNodeData.GridIdx = j;
                    grid.Initialize(gridNodeData);
                    grid.transform.localPosition = Vector3.zero;
                    grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                    circleData.GridNodes.Add(grid);
                    _circleIdxs[i].Add(j);

                    if((i == 0 && j == 3) || (i == 2 && j == 11) || (i == 3 && j == 5))
                    {
                        grid.CreateCharacter(GoalColor.Blue, gridParentObject.DoorTransform);
                    }

                    if((i == 0 && j == 1) || (i == 2 && j == 3))
                    {
                        grid.CreateCharacter(GoalColor.Green, gridParentObject.DoorTransform);
                    }
                }
            }
            _circleGridsParentById.Add(i, circleData);
        }
    }

    public void RotateCircle(int circleIdx, float angle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;

        _circleGridsParentById[circleIdx].CircleTransform.Rotate(Vector3.up, angle, Space.World);
    }

    public void StartRotateCircle(int circleIdx)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;
        foreach (var grid in _circleGridsParentById[circleIdx].GridNodes)
        {
            grid.SetSelectedColor(true);
        }
    }

    public void StopRotateCircle(int circleIdx, float totalAngle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;
        
        var angle = (360 / ONE_CIRCLE_GRID_COUNT);

        _circleGridsParentById[circleIdx].CircleTransform.rotation = Quaternion.Euler(0, Mathf.Round(_circleGridsParentById[circleIdx].CircleTransform.eulerAngles.y / angle) * angle, 0);

        foreach (var grid in _circleGridsParentById[circleIdx].GridNodes)
        {
            grid.SetSelectedColor(false);
        }

        totalAngle = totalAngle % 360;
        var to = Mathf.RoundToInt(totalAngle / angle) * angle;
        var angleCount = Mathf.Abs(to / angle);
        

        if(totalAngle < 0)
        {
            //Listeyi saat yönünde angle count kadar kaydır
            _circleIdxs[circleIdx] = 
                _circleIdxs[circleIdx]
                    .Skip(ONE_CIRCLE_GRID_COUNT - angleCount)
                    .Concat(_circleIdxs[circleIdx].Take(ONE_CIRCLE_GRID_COUNT - angleCount))
                    .ToList();
        }
        else
        {
            //Listeyi saat yönünün tersine angle count kadar kaydır
            _circleIdxs[circleIdx] =
                _circleIdxs[circleIdx]
                    .Skip(angleCount)
                    .Concat(_circleIdxs[circleIdx].Take(angleCount))
                    .ToList();
        }

        for(int i = 0; i < _circleGridsParentById[circleIdx].GridNodes.Count; i++)
        {
            _circleGridsParentById[circleIdx].GridNodes[i].UpdateGridIdx(_circleIdxs[circleIdx][i]);
        }

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.GridUpdated());
    }

    public void ResetGrid()
    {
        int i = 0;
        foreach (var circle in _circleGridsParentById.Values)
        {
            foreach (var grid in circle.GridNodes)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"GridLevel_{i+1}", grid);
            }
            i++;
        }

        foreach (var circleGrid in _circleGridsParentById.Values)
        {
            circleGrid.CircleTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        _circleGridsParentById.Clear();
        _circleIdxs.Clear();
    }

    public bool CheckAnyObstacle(int circleIdx, int gridIdx)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return false;
        if(_circleGridsParentById[circleIdx].GridNodes[gridIdx].GridNodeData.GridType == GridType.Empty) return true;
        else return false;
    }
}

public partial class Events
{
    public struct GridUpdated : IEvent
    {
    }
}

public class CircleData
{
    public Transform CircleTransform;
    public bool IsCircleWater;
    public List<GridNode> GridNodes;
}