using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : IManager
{
    public int MovementCount => _movementProvider.MovementCount;
    private IMovementProvider _movementProvider;
    public IManager CreateSelf()
    {
        return new MovementManager();
    }

    public void Initialize(GameInstaller gameInstaller, System.Action onReady)
    {
        _movementProvider = MovementProviderFactory.Create(gameInstaller.Customizer.MovementProvider);
        _movementProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _movementProvider != null;
    }
}