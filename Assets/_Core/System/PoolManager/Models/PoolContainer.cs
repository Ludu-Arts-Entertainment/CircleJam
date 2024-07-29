using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolContainer", menuName = "ScriptableObjects/PoolContainer", order = 1)]
public class PoolContainer : ScriptableObject
{
    public List<PoolData> PoolData = new List<PoolData>();
}