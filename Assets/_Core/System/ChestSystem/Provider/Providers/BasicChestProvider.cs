using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicChestProvider : IChestProvider
{
    private List<ChestData> _chestDataList;
    public IChestProvider CreateSelf()
    {
        return new BasicChestProvider();
    }

    public void Initialize(Action onReady)
    {
        _chestDataList = new List<ChestData>();
        _chestDataList = Resources.Load<ChestContainer>("ChestContainer")?.ChestDataList;
        onReady?.Invoke();
    }
    public ChestData GetChestData(string chestType)
    {
        return _chestDataList?.Find(x => x.ChestType == chestType);
    }

    public List<ProductBlock> Open(string chestType)
    {
        var chestData = GetChestData(chestType);
        List<ProductBlock> productBlocks = new List<ProductBlock>(chestData.WillGiveCount);
        for (int i = 0; i < chestData.WillGiveCount; i++)
        {
            productBlocks.Add(chestData.GetRandomProductBlock());
        }
        GiverService.Give(productBlocks,null);
        return productBlocks;
    }
}
