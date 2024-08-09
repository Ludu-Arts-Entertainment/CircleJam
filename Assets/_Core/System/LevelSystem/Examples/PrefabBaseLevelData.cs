using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabBaseLevelData : ILevelData
{
    [SerializeField]
    private int _index;
    public int Index => _index;
    public string Name;
    public string PrefabPoolId;
    public CircleJamLevelData LevelData;
}