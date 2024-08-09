using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJamLevelData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class LevelGridData
{
    public GridType gridType;
    public FixedPathType fixedPathType;
    public InteractablePathType interactablePathType;
    public FixedObstacleType fixedObstacleType;
    public bool hasCharacter;
    public GoalColor characterColor;

    public void Reset()
    {
        gridType = GridType.Normal;
        hasCharacter = false;
        characterColor = GoalColor.None;
    }
}
