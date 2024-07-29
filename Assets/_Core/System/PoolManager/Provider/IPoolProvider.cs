using System;
using UnityEngine;

public interface IPoolProvider
{
    IPoolProvider CreateSelf();
    void Initialize(Action onReady);
    T Instantiate<T>(string poolId, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component;
    void Destroy(string poolId, Component component);
    void DestroyAll();
    void DestroyAll(PoolTags tag);
}
