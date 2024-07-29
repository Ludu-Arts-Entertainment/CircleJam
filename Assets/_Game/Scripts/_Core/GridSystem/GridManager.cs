using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : IManager
{
    private IGridProvider _gridProvider;
    public IManager CreateSelf()
    {
        return new GridManager();
    }

    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _gridProvider = GridProviderFactory.Create(gameInstaller.Customizer.GridProvider);
        _gridProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _gridProvider != null;
    }
}