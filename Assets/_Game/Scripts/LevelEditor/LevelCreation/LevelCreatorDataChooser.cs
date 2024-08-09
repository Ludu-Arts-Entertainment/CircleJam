using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class LevelCreatorDataChooser : MonoBehaviour
{
    [Title("Grid Types")]
    [EnumToggleButtons,HideLabel] public GridType gridType;

    [Title("FixedPath Type")]
    [ShowIf("gridType",GridType.FixedPath)]
    [EnumToggleButtons,HideLabel]  public FixedPathType fixedPathType;

    [Title("InteractablePath Type")]
    [ShowIf("gridType",GridType.InteractablePath)]
    [EnumToggleButtons,HideLabel]  public InteractablePathType interactablePathType;

    [Title("FixedObstacle Type")]
    [ShowIf("gridType",GridType.FixedObstacle)]
    [EnumToggleButtons,HideLabel]  public FixedObstacleType fixedObstacleType;

    [Title("Has Character")]
    public bool hasCharacter;

    [Title("Character Color")]
    [ShowIf("hasCharacter")]
    [EnumToggleButtons,HideLabel] public GoalColor characterColor;
}