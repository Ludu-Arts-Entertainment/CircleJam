using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public int GridLevel => gridLevel;
    private int gridLevel;
    public void Initialize(int gridLevel)
    {
        this.gridLevel = gridLevel;
    }
}
