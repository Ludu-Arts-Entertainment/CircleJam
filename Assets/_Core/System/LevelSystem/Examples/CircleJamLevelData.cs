using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleJamLevelData", menuName = "ScriptableObjects/CircleJamLevelData", order = 1)]
public class CircleJamLevelData : ScriptableObject
{
    public List<LevelCircleData> CircleDataList = new List<LevelCircleData>();
}

[Serializable]
public class LevelGridData
{
    public GridType gridType;
    [ShowIf("gridType", GridType.FixedPath)] public FixedPathType fixedPathType;
    [ShowIf("gridType", GridType.InteractablePath)] public InteractablePathType interactablePathType;
    [ShowIf("gridType", GridType.FixedObstacle)] public FixedObstacleType fixedObstacleType;
    public bool hasCharacter;
    [ShowIf("hasCharacter")] public GoalColor characterColor;

    public void Reset()
    {
        gridType = GridType.Normal;
        hasCharacter = false;
        characterColor = GoalColor.None;
    }
}

[Serializable]
public class LevelCircleData
{
    public int CircleIndex;
    public List<LevelGridData> GridData;
}
