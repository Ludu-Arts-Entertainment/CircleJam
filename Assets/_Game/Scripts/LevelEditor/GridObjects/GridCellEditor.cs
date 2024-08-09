using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellEditor : MonoBehaviour
{
    public int circleLevel;
    public Collider collider;
    public Transform pivotPoint;
    public List<GridObjectEditor> gridObjs = new List<GridObjectEditor>();
    public LevelGridData gridData =new();


    public void RemoveObject()
    {
        gridData.Reset();
        if (gridObjs == null) return;
        foreach (var gridObj in gridObjs)
        {
            gridObj.Remove();
        }
        gridObjs = null;
    }
}