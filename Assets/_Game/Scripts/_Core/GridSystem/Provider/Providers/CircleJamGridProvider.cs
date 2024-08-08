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

    private CircleParent circleParentObject;
    public void CreateGrid(int circleCount, Transform parent)
    {
        if(circleParentObject == null)
        {
            circleParentObject = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<CircleParent>("CircleParent", parent: parent);
        }

        for (int i = 0; i < circleCount; i++)
        {
            var circleData = new CircleData();
            circleData.Circle = circleParentObject.CircleParents[i];
            circleData.GridNodes = new List<GridNode>();
            
            _circleIdxs.Add(i, new List<int>());

            if(i == 1)
            {
                circleData.IsCircleWater = true;
                var waterObject = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<Transform>($"WaterLevel_{i+1}", parent: circleData.Circle.NotRotateTransform);
                waterObject.localPosition = Vector3.zero;

                for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
                {
                    var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}");
                    var gridNodeData = new GridNodeData();
                    
                    if(j == 8)
                    {
                        gridNodeData.GridType = GridType.FixedPath;
                        gridNodeData.FixedPathType = FixedPathType.Bridge;
                        grid.transform.parent = circleData.Circle.NotRotateTransform;
                    }
                    else if(j == 11)
                    {
                        gridNodeData.GridType = GridType.InteractablePath;
                        gridNodeData.InteractablePathType = InteractablePathType.Sandal;
                        grid.transform.parent = circleData.Circle.RotateTransform;
                    }
                    else
                    {
                        gridNodeData.GridType = GridType.Empty;
                        grid.transform.parent = circleData.Circle.NotRotateTransform;
                    }
                    gridNodeData.CircleLevel = i;
                    gridNodeData.GridIdx = j;
                    grid.transform.localPosition = Vector3.zero;
                    grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                    grid.Initialize(circleData, gridNodeData);
                    circleData.GridNodes.Add(grid);
                    _circleIdxs[i].Add(j);
                    
                }
            }
            else
            {
                circleData.IsCircleWater = false;

                for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
                {
                    var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}", parent: circleData.Circle.RotateTransform);
                    var gridNodeData = new GridNodeData();

                    /*if(i == 2 && j == 7)
                    {
                        gridNodeData.GridType = GridType.FixedObstacle;
                        gridNodeData.FixedObstacleType = FixedObstacleType.Fence;
                    }
                    else*/
                    {
                        gridNodeData.GridType = GridType.Normal;
                    }

                    gridNodeData.CircleLevel = i;
                    gridNodeData.GridIdx = j;
                    grid.transform.localPosition = Vector3.zero;
                    grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                    grid.Initialize(circleData, gridNodeData);
                    circleData.GridNodes.Add(grid);
                    _circleIdxs[i].Add(j);

                    if((i == 0 && j == 3) || (i == 2 && j == 11) || (i == 3 && j == 5))
                    {
                        grid.CreateCharacter(GoalColor.Blue, circleParentObject.DoorTransform);
                    }
                    
                    if((i == 0 && j == 1) || (i == 2 && j == 3))
                    {
                        grid.CreateCharacter(GoalColor.Green, circleParentObject.DoorTransform);
                    }
                }
            }

            _circleGridsParentById.Add(i, circleData);
        }
    }

    public void RotateCircle(int circleIdx, float angle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;

        _circleGridsParentById[circleIdx].Circle.RotateTransform.Rotate(Vector3.up, angle, Space.World);
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

        _circleGridsParentById[circleIdx].Circle.RotateTransform.rotation = Quaternion.Euler(0, Mathf.Round(_circleGridsParentById[circleIdx].Circle.RotateTransform.eulerAngles.y / angle) * angle, 0);

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

        //_circleIdxs listesinde _circleGridsParentById[circleIdx].GridNodes[i].GridNodeData.GridType Fixed path olanı listeden çıkar
        var coppiedIdx = new List<int>(_circleIdxs[circleIdx]);
        for (int i = 0; i < _circleIdxs[circleIdx].Count; i++)
        {
            var gridType = _circleGridsParentById[circleIdx].GridNodes.FirstOrDefault(x => x.GridNodeData.GridIdx == _circleIdxs[circleIdx][i])?.GridNodeData.GridType;
            if(gridType == GridType.FixedPath || gridType == GridType.FixedObstacle)
            {
                coppiedIdx.Remove(_circleIdxs[circleIdx][i]);
            }
        }

        var index = 0;
        for(int i = 0; i < _circleGridsParentById[circleIdx].GridNodes.Count; i++)
        {
            var gridType = _circleGridsParentById[circleIdx].GridNodes[i].GridNodeData.GridType;
            if(gridType == GridType.FixedPath || gridType == GridType.FixedObstacle) continue;
            _circleGridsParentById[circleIdx].GridNodes[i].UpdateGridIdx(coppiedIdx[index]);
            index++;
        }

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.GridUpdated());
    }

    public void ResetGrid()
    {
        int i = 0;
        foreach (var circle in _circleGridsParentById.Values)
        {
            if(circle.IsCircleWater)
            {
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"WaterLevel_{i+1}", circle.Circle.NotRotateTransform.GetChild(0));
            }
            foreach (var grid in circle.GridNodes)
            {
                grid.ResetGrid();
                GameInstaller.Instance.SystemLocator.PoolManager.Destroy($"GridLevel_{i+1}", grid);
            }
            i++;
        }

        foreach (var circleGrid in _circleGridsParentById.Values)
        {
            circleGrid.Circle.RotateTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        _circleGridsParentById.Clear();
        _circleIdxs.Clear();
    }

    public bool CheckAnyObstacle(int circleIdx, int gridIdx)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return false;
        
        var gridType = _circleGridsParentById[circleIdx].GridNodes.FirstOrDefault(x => x.GridNodeData.GridIdx == gridIdx)?.GridNodeData.GridType;
        if(gridType == GridType.Empty || gridType == GridType.FixedObstacle) return true;
        else return false;
    }

    public bool CheckRotateObstacle(int circleIdx, int gridIdx, float totalAngle, float angleDifference)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return false;
        if(angleDifference > 0 || angleDifference < 0)
        {
            var angle = (360 / ONE_CIRCLE_GRID_COUNT);
            
            int total = 0;
            if(totalAngle > 0)
            {
                totalAngle -= 15f;
                var to = Mathf.RoundToInt(totalAngle / angle) * angle;
                int angleCount = Mathf.Abs(to / angle);
                total = gridIdx + angleCount + 1;
                if(total >= ONE_CIRCLE_GRID_COUNT)
                {
                    total -= ONE_CIRCLE_GRID_COUNT;
                }

                var grid = _circleGridsParentById[circleIdx].GridNodes.FirstOrDefault(x => x.GridNodeData.GridIdx == total);
                if(grid == null) return false;
                if(total == gridIdx) return false;
                if(grid.GridNodeData.GridIdx > gridIdx && angleDifference < 0) return false;

                if(grid.GridNodeData.GridType == GridType.InteractablePath || grid.GridNodeData.GridType == GridType.FixedPath)
                {
                    return true;
                }
            }
            else
            {
                totalAngle += 15f;
                var to = Mathf.RoundToInt(totalAngle / angle) * angle;
                int angleCount = Mathf.Abs(to / angle);
                total = gridIdx - angleCount - 1;
                if(total < 0)
                {
                    total += ONE_CIRCLE_GRID_COUNT;
                }

                var grid = _circleGridsParentById[circleIdx].GridNodes.FirstOrDefault(x => x.GridNodeData.GridIdx == total);
                if(grid == null) return false;
                if(total == gridIdx) return false;
                if(grid.GridNodeData.GridIdx < gridIdx && angleDifference > 0) return false;

                if(grid.GridNodeData.GridType == GridType.InteractablePath || grid.GridNodeData.GridType == GridType.FixedPath)
                {
                    return true;
                }
            }
            
            return false;
        }

        return false;
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
    public Circle Circle;
    public bool IsCircleWater;
    public List<GridNode> GridNodes;
}