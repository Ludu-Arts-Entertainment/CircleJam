#if !LevelManager_Modified
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelContainer", menuName = "ScriptableObjects/LevelContainer", order = 1)]
public class LevelContainer : ScriptableObject
{
    public List<PrefabBaseLevelData> LevelDataList = new List<PrefabBaseLevelData>();

    public PrefabBaseLevelData GetLevelData(int levelIndex)
    {
        var i = LevelDataList.FindIndex(x => x.Index == levelIndex);
        if (i != -1)
        {
            return LevelDataList[i];
        }
        Debug.Log("LevelData not found! Key: " + levelIndex);
        return null;
    }
}
#endif