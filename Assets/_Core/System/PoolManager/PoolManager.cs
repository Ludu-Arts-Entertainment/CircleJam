using System;
using UnityEngine;

public class PoolManager : IManager
{
    private IPoolProvider _poolProvider;
    public IManager CreateSelf()
    {
        return new PoolManager();
    }
    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _poolProvider = PoolProviderFactory.Create(gameInstaller.Customizer.PoolProvider);
        _poolProvider.Initialize(onReady.Invoke);
    }

    public bool IsReady()
    {
        return _poolProvider != null;
    }

    public T Instantiate<T>(string poolId, Vector3 position = default, Quaternion rotation = default,
        Transform parent = null) where T : Component
    {
        return _poolProvider.Instantiate<T>(poolId, position, rotation, parent);
    }
    public void Destroy(string poolId, Component component)
    {
        _poolProvider.Destroy(poolId, component);
    }

    public void DestroyAll()
    {
        _poolProvider.DestroyAll();
    }
    public void DestroyAll(PoolTags tag)
    {
        _poolProvider.DestroyAll(tag);
    }
}
