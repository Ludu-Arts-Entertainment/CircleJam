#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    private SceneVisibilityManager sv;

    private LevelCreatorDataChooser data;
    [Header("Goal Color Order")]
    public List<GoalColor> goalColorsOrder;

    [Header("Objects")]
    public CharacterEditor characterPrefab;
    public FixedObstacleEditor fixedObstacle;

    public void AddObject(GridCellEditor cellEditor)
    {
        data ??= GetComponent<LevelCreatorDataChooser>();
        if (cellEditor.gridObjs!=null)
            cellEditor.RemoveObject();
        
        cellEditor.gridObjs = new List<GridObjectEditor>();

        if (data.gridType == GridType.Normal)
        {
            #region Data
            cellEditor.gridData.gridType = data.gridType;
            #endregion
        }   

        if(data.gridType == GridType.FixedObstacle)
        {
            var obstacle = Instantiate(fixedObstacle, cellEditor.transform);
            obstacle.CreateObject(data.fixedObstacleType, cellEditor.circleLevel);
            cellEditor.gridObjs.Add(obstacle);

            #region Data
            cellEditor.gridData.gridType = data.gridType;
            cellEditor.gridData.fixedObstacleType = data.fixedObstacleType;
            #endregion
        }

        #region Character
        if(data.hasCharacter)
        {
            cellEditor.gridData.hasCharacter = true;
            cellEditor.gridData.characterColor = data.characterColor;

            var character = Instantiate(characterPrefab, cellEditor.transform);
            character.characterColor = data.characterColor;
            character.SetColor();

            character.transform.position = cellEditor.pivotPoint.position;
            character.transform.rotation = cellEditor.pivotPoint.rotation;
            
            character.transform.localScale = Vector3.one;
            character.name = "Character";
            cellEditor.gridObjs.Add(character.GetComponent<GridObjectEditor>());
        }
        #endregion


        #region DisablePicking
        if (sv==null)
            sv= SceneVisibilityManager.instance;
        sv.DisableAllPicking();
        #endregion
    }

    public void ClearAll()
    {
        var cells = FindObjectsOfType<GridCellEditor>().ToList();
        foreach (var gridCell in cells)
        {
            gridCell.RemoveObject();
        }
    }

    [Button]
    public void Reset()
    {
        var cells = FindObjectsOfType<GridCellEditor>().ToList();
        foreach (var gridCell in cells)
        {
            gridCell.RemoveObject();
        }
    }
}
#endif