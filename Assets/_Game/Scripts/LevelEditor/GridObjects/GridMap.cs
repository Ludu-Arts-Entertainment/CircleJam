using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] private List<GridCellEditor> cellObjectsEditor;
    [Button(ButtonSizes.Large)]
    public void Reset()
    {
        foreach (var cell in cellObjectsEditor)
        {
            cell.RemoveObject();
        }
    }
}
