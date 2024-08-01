using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleJamGridProvider : IGridProvider
{
    private const int ONE_CIRCLE_GRID_COUNT = 12;
    private Dictionary<int, List<int>> _circleIdxs = new ();
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
            _circleIdxs.Add(i, new List<int>());
            _circleGridsByParent.Add(gridParent.transform, new List<GridNode>());
            _circleGridsParentById.Add(i, gridParent.transform);

            for (int j = 0; j < ONE_CIRCLE_GRID_COUNT; j++)
            {
                var grid = GameInstaller.Instance.SystemLocator.PoolManager.Instantiate<GridNode>($"GridLevel_{i+1}", parent: gridParent.transform);
                grid.Initialize(i, j);
                grid.transform.localPosition = Vector3.zero;
                grid.transform.localRotation = Quaternion.Euler(0, j * (360 / ONE_CIRCLE_GRID_COUNT), 0);
                _circleGridsByParent[gridParent.transform].Add(grid);
                _circleIdxs[i].Add(j);

                /*if((i == 0 && j == 0)|| (i == 1 && j == 4) || (i == 2 && j == 8) || (i == 3 && j == 2))
                {
                    grid.CreateCharacter(GoalColors.Red, gridParentObject.DoorTransform);
                }*/

                if((i == 0 && j == 3) || (i == 1 && j == 7) || (i == 2 && j == 11) || (i == 3 && j == 5))
                {
                    grid.CreateCharacter(GoalColors.Blue, gridParentObject.DoorTransform);
                }

                if((i == 0 && j == 1) || (i == 1 && j == 6) || (i == 2 && j == 3))
                {
                    grid.CreateCharacter(GoalColors.Green, gridParentObject.DoorTransform);
                }
            }
        }

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.GoalUpdated(GameInstaller.Instance.SystemLocator.GoalManager.CurrentGoalCount, false));
    }

    public void RotateCircle(int circleIdx, float angle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;

        _circleGridsParentById[circleIdx].Rotate(Vector3.up, angle, Space.World);
    }

    public void StartRotateCircle(int circleIdx)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;
        foreach (var grid in _circleGridsByParent[_circleGridsParentById[circleIdx]])
        {
            grid.SetSelectedColor(true);
        }
    }

    public void StopRotateCircle(int circleIdx, float totalAngle)
    {
        if(!_circleGridsParentById.ContainsKey(circleIdx)) return;
        
        var angle = (360 / ONE_CIRCLE_GRID_COUNT);

        _circleGridsParentById[circleIdx].rotation = Quaternion.Euler(0, Mathf.Round(_circleGridsParentById[circleIdx].eulerAngles.y / angle) * angle, 0);

        foreach (var grid in _circleGridsByParent[_circleGridsParentById[circleIdx]])
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

        foreach(var grids in _circleGridsByParent.Values)
        {
            int i = 0;
            foreach(var grid in grids)
            {
                grid.UpdateGridIdx(_circleIdxs[grid.GridLevel][i]);
                i++;
            }
        }

        GameInstaller.Instance.SystemLocator.EventManager.Trigger(new Events.GridUpdated());

        /*for(int i = 0; i < 4; i++)
        {
            int j = 0;
            foreach(var grid in _circleGridsByParent[_circleGridsByParent.Keys.ToList()[i]])
            {
                if(grid.HaveCharacter)
                {
                    Debug.Log($"Character idx: {j} color: {grid.Character.Color}");
                }
                j++;
            }
        }*/
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

        foreach (var circleGrid in _circleGridsParentById)
        {
            circleGrid.Value.rotation = Quaternion.Euler(0, 0, 0);
        }

        _circleGridsByParent.Clear();   
        _circleGridsParentById.Clear();
        _circleIdxs.Clear();
    }
}

public partial class Events
{
    public struct GridUpdated : IEvent
    {
    }
}