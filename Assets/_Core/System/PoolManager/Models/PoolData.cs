using UnityEngine;

[System.Serializable]
public struct PoolData
{
    public string Id;
    public Component Prefab;
    public int InitialCount;
    public PoolTags Tag;
}
public enum PoolTags
{
    Level,
    UI,
    Meta
}