using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridNodeCollection", menuName = "ScriptableObjects/GridSystem/GridNodeCollection")]
public class GridNodeCollection : ScriptableObject
{
    [Header("Fixed Path")]
    public List<FixedPathData> FixedPathDatas;

    public string GetModelNameByFixedPathType(FixedPathType fixedPathType)
    {
        foreach (var fixedPathData in FixedPathDatas)
        {
            if (fixedPathData.FixedPathType == fixedPathType)
            {
                return fixedPathData.ModelName;
            }
        }

        return string.Empty;
    }
}

[Serializable]
public class FixedPathData
{
    public FixedPathType FixedPathType;
    public string ModelName;
}
