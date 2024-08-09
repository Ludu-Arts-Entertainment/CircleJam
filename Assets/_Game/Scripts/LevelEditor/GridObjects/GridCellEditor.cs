using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellEditor : MonoBehaviour
{
    public Collider collider;
    public Transform pivotPoint;
    public GridObjectEditor gridObj;
    public LevelGridData gridData =new();


    public void RemoveObject()
    {
        gridData.Reset();
        if (gridObj == null) return;
        gridObj.Remove();
        gridObj = null;
    }
}