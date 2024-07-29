using UnityEngine;

[CreateAssetMenu(fileName = "BasicLevelRequirement", menuName = "ScriptableObjects/Requirements/BasicLevelRequirement")]
public class BasicLevelRequirement : BaseRequirement
{
    public override bool Check(float minValue, float maxValue)
    {
        var currentLevelIndex = GameInstaller.Instance.SystemLocator.LevelManager.CurrentLevelIndex;
        return currentLevelIndex >= minValue && currentLevelIndex <= maxValue;
    }
}
