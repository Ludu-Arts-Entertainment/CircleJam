using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "RouletteContainer", menuName = "ScriptableObjects/RouletteContainer", order = 1)]
public class RouletteContainer : ScriptableObject
{
    public int RouletteElementCount;
    [OnValueChanged("OnRouletteDataListChanged")]
    public List<RouletteData> RouletteDatas = new List<RouletteData>();
    private Dictionary<int, RouletteData> _rouletteDataDictionary;
    public Dictionary<int, RouletteData> RouletteDataDictionary
    {
        get
        {
            if(_rouletteDataDictionary == null)
            {
                _rouletteDataDictionary = new Dictionary<int, RouletteData>();
                foreach (var rouletteData in RouletteDatas)
                {
                    _rouletteDataDictionary.TryAdd(rouletteData.Id, rouletteData);
                }
            }
            return _rouletteDataDictionary;
        }
    }

    private void OnRouletteDataListChanged()
    {
        if(RouletteDatas.Count == 0) return;
        RouletteDatas.Last().Id = RouletteDatas.Count;
    }
}

[Serializable]
public class RouletteData
{
    public int Id;
    public ProductBlockType ProductBlockType;
    public ProductBlockSubType ProductBlockSubType;
    public int MinAmount;
    public int MaxAmount;
}

public class RouletteSaveData
{
    public readonly int RouletteDataId;
    public bool IsGained;
    public readonly ProductBlock ProductBlock;
    public readonly string Id;
    public RouletteSaveData(int rouletteDataId, bool isGained, ProductBlock productBlock, string id)
    {
        RouletteDataId = rouletteDataId;
        IsGained = isGained;
        ProductBlock = productBlock;
        Id = id;
    }
}
