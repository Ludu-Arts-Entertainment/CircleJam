using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : IManager
{
    private IInventoryProvider _inventoryProvider;
    public IManager CreateSelf()
    {
        return new InventoryManager();
    }
    public event Action<string,string,string,InventoryItemChangeType> OnInventoryChanged
    {
        add => _inventoryProvider.OnInventoryChanged += value;
        remove => _inventoryProvider.OnInventoryChanged -= value;
    }
    public void Initialize(GameInstaller gameInstaller, Action onReady)
    {
        _inventoryProvider = InventoryProviderFactory.Create(gameInstaller.Customizer.InventoryProvider);
        _inventoryProvider.Initialize(onReady);
    }

    public bool IsReady()
    {
        return _inventoryProvider != null;
    }
    public void Add(string type, string id, IInventoryItemData data)
    {
        _inventoryProvider.Add(type,id,data);
    }
    public void Remove(string type, string id, string uuid)
    {
        _inventoryProvider.Remove(type,id,uuid);
    }
    public void Update(string type, string id, IInventoryItemData data)
    {
        _inventoryProvider.Update(type,id,data);
    }
    public void Increase(string type, string id, string uuid, float amount)
    {
        _inventoryProvider.Increase(type,id,uuid,amount);
    }
    public void Decrease(string type, string id, string uuid, float amount)
    {
        _inventoryProvider.Decrease(type,id,uuid,amount);
    }
    public void RemoveAll(string type)
    {
        _inventoryProvider.RemoveAll(type);
    }
    public void RemoveAll(string type, string id)
    {
        _inventoryProvider.RemoveAll(type,id);
    }
    public void RemoveAll(string type, string id, Func<IInventoryItemData,bool> comparer)
    {
        _inventoryProvider.RemoveAll(type,id,comparer);
    }
    public Dictionary<string, Dictionary<string, List<IInventoryItemData>>> Get()
    {
        return _inventoryProvider.Get();
    }
    public Dictionary<string,List<IInventoryItemData>> Get(string type)
    {
        return _inventoryProvider.Get(type);
    }
    public List<IInventoryItemData> Get(string type, string id)
    {
        return _inventoryProvider.Get(type,id);
    }
    public List<IInventoryItemData> Get(string type, string id, Func<IInventoryItemData,bool> comparer)
    {
        return _inventoryProvider.Get(type,id,comparer);
    }
    public List<string> GetHasInventoryTypes()
    {
        return _inventoryProvider.GetHasInventoryTypes();
    }
    public List<string> GetHasInventoryItemIds(string type)
    {
        return _inventoryProvider.GetHasInventoryItemIds(type);
    }
    public List<string> GetNoHasInventoryTypes()
    {
        return _inventoryProvider.GetNoHasInventoryTypes();
    }
    public List<string> GetNoHasInventoryItemIds(string type)
    {
        return _inventoryProvider.GetNoHasInventoryItemIds(type);
    }
}
