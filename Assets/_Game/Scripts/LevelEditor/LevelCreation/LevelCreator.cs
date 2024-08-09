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

    [Header("Objects")]
    public CharacterEditor characterPrefab;

    public void AddObject(GridCellEditor cellEditor)
    {
        data ??= GetComponent<LevelCreatorDataChooser>();
        if (cellEditor.gridObj!=null)
            cellEditor.RemoveObject();
        
        #region Player

        if (data.gridType==GridType.Normal)
        {
            
            
            #region Data
            cellEditor.gridData.gridType = data.gridType;
            
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
            character.name = "Player";
            cellEditor.gridObj = character.GetComponent<GridObjectEditor>();
        }
        #endregion

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
