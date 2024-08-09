using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public List<GridCellEditor> circle_1;
    public List<GridCellEditor> circle_2;
    public List<GridCellEditor> circle_3;
    public List<GridCellEditor> circle_4;

    [Button(ButtonSizes.Large)]
    public void Reset()
    {
        for(int i = 0; i < circle_1.Count; i++)
        {
            circle_1[i].RemoveObject();
            circle_2[i].RemoveObject();
            circle_3[i].RemoveObject();
            circle_4[i].RemoveObject();
        }
    }
}
