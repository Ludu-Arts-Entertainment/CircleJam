#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelCreatorHelper : MonoBehaviour
{
    private GridMap _gridMap;
    public int levelIndex;
    public CircleJamLevelData currentLevelData;

    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
    public void Save()
    {
        _gridMap ??= FindObjectOfType<GridMap>();
        currentLevelData = Resources.Load<CircleJamLevelData>($"Levels/Level_{levelIndex}");
        if (currentLevelData is null)
        {
            currentLevelData = ScriptableObject.CreateInstance<CircleJamLevelData>();
            AssetDatabase.CreateAsset(currentLevelData, $"Assets/_Game/Resources/Levels/Level_{levelIndex}.asset");
        }

        EditorUtility.SetDirty(currentLevelData);

        currentLevelData.CircleDataList = new List<LevelCircleData>();

        #region Circle_1
        var circleData_1 = new LevelCircleData();
        circleData_1.CircleIndex = 0;
        circleData_1.GridData = new List<LevelGridData>();

        foreach(var grid in _gridMap.circle_1)
        {
            var levelGridData = new LevelGridData();
            levelGridData.gridType = grid.gridData.gridType;
            levelGridData.fixedObstacleType = grid.gridData.fixedObstacleType;
            levelGridData.fixedPathType = grid.gridData.fixedPathType;
            levelGridData.interactablePathType = grid.gridData.interactablePathType;

            levelGridData.hasCharacter = grid.gridData.hasCharacter;
            levelGridData.characterColor = grid.gridData.characterColor;

            circleData_1.GridData.Add(levelGridData);
        }

        currentLevelData.CircleDataList.Add(circleData_1);
        #endregion

        #region Circle_2
        var circleData_2 = new LevelCircleData();
        circleData_2.CircleIndex = 1;
        circleData_2.GridData = new List<LevelGridData>();

        foreach(var grid in _gridMap.circle_2)
        {
            var levelGridData = new LevelGridData();
            levelGridData.gridType = grid.gridData.gridType;
            levelGridData.fixedObstacleType = grid.gridData.fixedObstacleType;
            levelGridData.fixedPathType = grid.gridData.fixedPathType;
            levelGridData.interactablePathType = grid.gridData.interactablePathType;

            levelGridData.hasCharacter = grid.gridData.hasCharacter;
            levelGridData.characterColor = grid.gridData.characterColor;

            circleData_2.GridData.Add(levelGridData);
        }

        currentLevelData.CircleDataList.Add(circleData_2);
        #endregion

        #region Circle_3
        var circleData_3 = new LevelCircleData();
        circleData_3.CircleIndex = 2;
        circleData_3.GridData = new List<LevelGridData>();

        foreach(var grid in _gridMap.circle_3)
        {
            var levelGridData = new LevelGridData();
            levelGridData.gridType = grid.gridData.gridType;
            levelGridData.fixedObstacleType = grid.gridData.fixedObstacleType;
            levelGridData.fixedPathType = grid.gridData.fixedPathType;
            levelGridData.interactablePathType = grid.gridData.interactablePathType;

            levelGridData.hasCharacter = grid.gridData.hasCharacter;
            levelGridData.characterColor = grid.gridData.characterColor;

            circleData_3.GridData.Add(levelGridData);
        }

        currentLevelData.CircleDataList.Add(circleData_3);
        #endregion

        #region Circle_4
        var circleData_4 = new LevelCircleData();
        circleData_4.CircleIndex = 3;
        circleData_4.GridData = new List<LevelGridData>();

        foreach(var grid in _gridMap.circle_4)
        {
            var levelGridData = new LevelGridData();
            levelGridData.gridType = grid.gridData.gridType;
            levelGridData.fixedObstacleType = grid.gridData.fixedObstacleType;
            levelGridData.fixedPathType = grid.gridData.fixedPathType;
            levelGridData.interactablePathType = grid.gridData.interactablePathType;

            levelGridData.hasCharacter = grid.gridData.hasCharacter;
            levelGridData.characterColor = grid.gridData.characterColor;

            circleData_4.GridData.Add(levelGridData);
        }

        currentLevelData.CircleDataList.Add(circleData_4);
        #endregion

        Debug.Log($"Level {levelIndex} Saved!");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif