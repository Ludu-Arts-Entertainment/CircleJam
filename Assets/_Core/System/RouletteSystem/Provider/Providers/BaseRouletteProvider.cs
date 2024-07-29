using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRouletteProvider : IRouletteProvider
{
    private Dictionary<int, RouletteSaveData> _createdRouletteDatas = new();
    private RouletteContainer _rouletteContainer;

    public IRouletteProvider CreateSelf()
    {
        return new BaseRouletteProvider();
    }

    public void Initialize(Action onReady)
    {
        _rouletteContainer = Resources.Load<RouletteContainer>("RouletteContainer");
        onReady?.Invoke();
    }

    public Dictionary<int, RouletteSaveData> CreateNewRouletteDatas()
    {
        _createdRouletteDatas.Clear();
        var rouletteItemKeys = _rouletteContainer?.RouletteDatas?.ConvertAll(x => x.Id);

        for (int i = 0; i < _rouletteContainer?.RouletteElementCount; i++)
        {
            var randomRouletteId = rouletteItemKeys[UnityEngine.Random.Range(0, rouletteItemKeys.Count)];
            var randomRouletteData = _rouletteContainer.RouletteDataDictionary[randomRouletteId];
            if (_createdRouletteDatas.ContainsKey(randomRouletteData.Id))
            {
                i--;
                continue;
            }
            var amount = UnityEngine.Random.Range(randomRouletteData.MinAmount, randomRouletteData.MaxAmount);
            _createdRouletteDatas.Add(randomRouletteData.Id,
                new RouletteSaveData
                (
                    rouletteDataId: randomRouletteData.Id,
                    isGained: false,
                    productBlock: new ProductBlock(type: randomRouletteData.ProductBlockType, subType: randomRouletteData.ProductBlockSubType, amount: amount),
                    id: randomRouletteData.ProductBlockSubType.ToString()
                ));

            rouletteItemKeys.Remove(randomRouletteId);
        }

        return _createdRouletteDatas;
    }
}