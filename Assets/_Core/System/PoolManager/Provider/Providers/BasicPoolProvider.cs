using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class BasicPoolProvider : IPoolProvider
{
    private PoolContainer[] _poolContainers;
    private Dictionary<string, Queue<Component>> _freePoolObjects = new();
    private Dictionary<string, List<Component>> _busyPoolObjects = new();
    private Transform PoolParent;
    private List<PoolData> allPoolData = new List<PoolData>();

    public IPoolProvider CreateSelf()
    {
        return new BasicPoolProvider();
    }

    public void Initialize(Action onReady)
    {
        PoolParent = new GameObject("PoolParent").transform;
        GameObject.DontDestroyOnLoad(PoolParent);
        _poolContainers = Resources.LoadAll<PoolContainer>("PoolContainers");
        allPoolData = _poolContainers.SelectMany(x => x.PoolData).ToList();
        foreach (var poolData in allPoolData)
        {
            for (int i = 0; i < poolData.InitialCount; i++)
            {
                _freePoolObjects.TryAdd(poolData.Id, new Queue<Component>());
                _freePoolObjects[poolData.Id].Enqueue(CreateItem<Component>(poolData));
            }
        }
        onReady?.Invoke();
    }

    public T Instantiate<T>(string poolId, Vector3 position = default, Quaternion rotation = default,
        Transform parent = null) where T : Component
    {
        if (_freePoolObjects.TryGetValue(poolId, out var queue))
        {
            if (queue.TryDequeue(out Component result))
            {
                _busyPoolObjects.TryAdd(poolId, new List<Component>());
                _busyPoolObjects[poolId].Add(result);
                Spawn(result, position, rotation, parent);
                return result as T;
            }
            var component = CreateItem<T>(allPoolData.FirstOrDefault(x => x.Id == poolId));
            _busyPoolObjects.TryAdd(poolId, new List<Component>());
            _busyPoolObjects[poolId].Add(component);
            Spawn(component, position, rotation, parent);
            return component;
        }
        var poolIndex = allPoolData.FindIndex(x => x.Id == poolId);
        if (poolIndex == -1)
        {
            Debug.LogError($"Pool with id {poolId} not found");
            return null;
        }
        var createdComponent = CreateItem<T>(allPoolData[poolIndex]);
        _busyPoolObjects.TryAdd(poolId, new List<Component>());
        _busyPoolObjects[poolId].Add(createdComponent);
        Spawn(createdComponent, position, rotation, parent);
        return createdComponent;
    }

    public void Destroy(string poolId, Component component)
    {
        if (_busyPoolObjects.TryGetValue(poolId, out var list))
        {
            var index = list.FindIndex(x => x == component);
            component.gameObject.SetActive(false);
            component.transform.SetParent(PoolParent);
            if (component.TryGetComponent(out IPoolObject poolObject))
            {
                poolObject.OnRecycled();
            }
            list.RemoveAt(index);
            _freePoolObjects.TryAdd(poolId, new Queue<Component>());
            _freePoolObjects[poolId].Enqueue(component);
        }
    }

    public void DestroyAll()
    {
        var poolObjectLists = new Dictionary<string, List<Component>>();
        foreach (var pair in _busyPoolObjects)
        {
            poolObjectLists.Add(pair.Key, pair.Value);
        }
        foreach (var poolObjectList in poolObjectLists)
        {
            while (poolObjectList.Value.Count!=0)
            {
                Destroy(poolObjectList.Key, poolObjectList.Value[0]);
            }
        }
    }

    public void DestroyAll(PoolTags tag)
    {
        var poolObjectLists = new Dictionary<string, List<Component>>();
        foreach (var pair in _busyPoolObjects)
        {
            poolObjectLists.Add(pair.Key, pair.Value);
        }
        foreach (var poolObjectList in poolObjectLists)
        {
            var poolData = allPoolData.FirstOrDefault(x => x.Id == poolObjectList.Key);
            if (poolData.Tag == tag)
            {
                while (poolObjectList.Value.Count!=0)
                {
                    Destroy(poolObjectList.Key, poolObjectList.Value[0]);
                }
            }
        }
    }

    private T CreateItem<T>(PoolData poolData) where T : Component
    {
        var poolObject = Object.Instantiate(poolData.Prefab, PoolParent);
        var component = poolObject.GetComponent(poolData.Prefab.GetType());
        poolObject.gameObject.SetActive(false);
        return component as T;
    }
    private void Spawn(Component component, Vector3 position = default, Quaternion rotation = default,
        Transform parent = null)
    {
        component.transform.SetParent(parent == null ? PoolParent : parent);
        component.transform.position = position;
        component.transform.rotation = rotation;
        component.gameObject.SetActive(true);
        if (component.TryGetComponent(out IPoolObject poolObject))
        {
            poolObject.OnSpawned();
        }
    }
}