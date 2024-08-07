using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridNodeCollectionService
{
    private static bool _isReady;
    private static GridNodeCollection _gridNodeCollection;
    private static void Initialize()
    {
        _gridNodeCollection = Resources.Load<GridNodeCollection>("GridNodeCollection");
        _isReady = true;
    }

    public static string GetModelNameByFixedPathType(FixedPathType fixedPathType)
    {
        if (_isReady) return _gridNodeCollection.GetModelNameByFixedPathType(fixedPathType);
        Initialize();
        return _gridNodeCollection.GetModelNameByFixedPathType(fixedPathType);
    }

    public static string GetModelNameByInteractablePathType(InteractablePathType interactablePathType)
    {
        if (_isReady) return _gridNodeCollection.GetModelNameByInteractablePathType(interactablePathType);
        Initialize();
        return _gridNodeCollection.GetModelNameByInteractablePathType(interactablePathType);
    }

    public static string GetModelNameByFixedObstacleType(FixedObstacleType fixedObstacleType)
    {
        if (_isReady) return _gridNodeCollection.GetModelNameByFixedObstacleType(fixedObstacleType);
        Initialize();
        return _gridNodeCollection.GetModelNameByFixedObstacleType(fixedObstacleType);
    }
}
