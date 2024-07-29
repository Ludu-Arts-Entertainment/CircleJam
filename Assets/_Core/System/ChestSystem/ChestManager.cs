using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : IManager
{
    private IChestProvider _chestProvider;
    public IManager CreateSelf()
    {
        return new ChestManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _chestProvider = ChestProviderFactory.Create(gameInstaller.Customizer.ChestProvider);
        _chestProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _chestProvider != null;
    }
    public ChestData GetChestData(string chestType)
    {
        return _chestProvider.GetChestData(chestType);
    }
    public List<ProductBlock> Open(string chestType)
    {
        return _chestProvider.Open(chestType);
    }
}
