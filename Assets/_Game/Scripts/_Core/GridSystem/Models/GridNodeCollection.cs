using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridNodeCollection", menuName = "ScriptableObjects/GridSystem/GridNodeCollection")]
public class GridNodeCollection : ScriptableObject
{
    [Header("Fixed Path")]
    public List<FixedPathData> FixedPathDatas;

    [Header("Interactable Path")]
    public List<InteractablePathData> InteractablePathDatas;

    [Header("Fixed Obstacle")]
    public List<FixedObstacleData> FixedObstacleDatas;

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

    public string GetModelNameByInteractablePathType(InteractablePathType interactablePathType)
    {
        foreach (var interactablePathData in InteractablePathDatas)
        {
            if (interactablePathData.InteractablePathType == interactablePathType)
            {
                return interactablePathData.ModelName;
            }
        }

        return string.Empty;
    }

    public string GetModelNameByFixedObstacleType(FixedObstacleType fixedObstacleType)
    {
        foreach (var fixedObstacleData in FixedObstacleDatas)
        {
            if (fixedObstacleData.FixedObstacleType == fixedObstacleType)
            {
                return fixedObstacleData.ModelName;
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

[Serializable]
public class InteractablePathData
{
    public InteractablePathType InteractablePathType;
    public string ModelName;
}

[Serializable]
public class FixedObstacleData
{
    public FixedObstacleType FixedObstacleType;
    public string ModelName;
}