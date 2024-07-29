using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicInventoryProvider : IInventoryProvider
{
    private Dictionary<string,Dictionary<string,List<IInventoryItemData>>> _inventoryData = new ();
    public IInventoryProvider CreateSelf()
    {
        return new BasicInventoryProvider();
    }

    public event Action<string, string, string, InventoryItemChangeType> OnInventoryChanged;

    public void Initialize(Action onReady)
    {
        Load();
        onReady?.Invoke();
    }

    private void Load()
    {
        var inventoryData = GameInstaller.Instance.SystemLocator.DataManager.GetData<Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>>>(GameDataType.InventoryData);
        foreach (var inventoryType in inventoryData)
        {
            _inventoryData.TryAdd(inventoryType.Key,new Dictionary<string, List<IInventoryItemData>>() );
            foreach (var item in inventoryType.Value)
            {
                _inventoryData[inventoryType.Key].TryAdd(item.Key,new List<IInventoryItemData>());
                foreach (var data in item.Value)
                {
                    foreach (var pair in data.Value)
                    {
                        var inventoryItemData = JsonHelper.FromJson<IInventoryItemData>(pair, data.Key);
                        _inventoryData[inventoryType.Key][item.Key].Add(inventoryItemData);
                    }
                }
            }
        }
    }

    private void Save()
    {
        Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>> inventoryData = new ();
        foreach (var inventoryType in _inventoryData)
        {
            inventoryData.TryAdd(inventoryType.Key,new Dictionary<string, Dictionary<Type, List<string>>>());
            foreach (var item in inventoryType.Value)
            {
                inventoryData[inventoryType.Key].TryAdd(item.Key,new Dictionary<Type, List<string>>());
                foreach (var data in item.Value)
                {
                    inventoryData[inventoryType.Key][item.Key].TryAdd(data.GetType(),new List<string>());
                    inventoryData[inventoryType.Key][item.Key][data.GetType()].Add(JsonHelper.ToJson(data));
                }
            }
        }
        GameInstaller.Instance.SystemLocator.DataManager.SetData(GameDataType.InventoryData,inventoryData);
        GameInstaller.Instance.SystemLocator.DataManager.SaveData();
    }
    
    
    public void Add(string type, string id, IInventoryItemData data)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            _inventoryData.TryAdd(type,new Dictionary<string, List<IInventoryItemData>>());
        }
        _inventoryData[type].TryAdd(id,new List<IInventoryItemData>());
        data.SetId(TimeHelper.DateTimeToUnixTimeStampInSeconds(TimeHelper.GetCurrentDateTime())+"_" + UnityEngine.Random.Range(30,99999999));
        data.ItemId = id;
        _inventoryData[type][id].Add(data);
        Save();
        OnInventoryChanged?.Invoke(type,id,data.GetId(),InventoryItemChangeType.Add);
    }

    public void Update(string type, string id, IInventoryItemData data)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            _inventoryData.TryAdd(type,new Dictionary<string, List<IInventoryItemData>>());
        }
        _inventoryData[type].TryAdd(id,new List<IInventoryItemData>());
        var index =_inventoryData[type][id].FindIndex(x=>x.GetId()==data.GetId());
        data.ItemId = id;
        if(index==-1)
        {
            Add(type,id,data);
        }else
            _inventoryData[type][id][index] = data;
        Save();
        OnInventoryChanged?.Invoke(type,id,data.GetId(),InventoryItemChangeType.Update);

    }

    public void Increase(string type, string id, string uuid, float amount)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var index = _inventoryData[type][id].FindIndex(x=>x.GetId()==uuid);
        if (index != -1)
        {
            _inventoryData[type][id][index].Amount += Mathf.Abs(amount);        
            Save();

            OnInventoryChanged?.Invoke(type,id,uuid,InventoryItemChangeType.Update);
        }
        else
        {
            Debug.LogWarning("Item not found in inventory");
        }
    }

    public void Decrease(string type, string id, string uuid, float amount)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var index = _inventoryData[type][id].FindIndex(x=>x.GetId()==uuid);
        if (index != -1)
        {
            _inventoryData[type][id][index].Amount += -Mathf.Abs(amount);
            if (_inventoryData[type][id][index].Amount<=0)
            {
                Remove(type,id,uuid);
                return;
            }
            Save();
            OnInventoryChanged?.Invoke(type,id,uuid,InventoryItemChangeType.Update);

        }
        else
        {
            Debug.LogWarning("Item not found in inventory");
        }
    }


    public void Remove(string type, string id, string uuid)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var index = _inventoryData[type][id].FindIndex(x=>x.GetId()==uuid);
        if (index != -1)
        {
            _inventoryData[type][id].RemoveAt(index);
            if (_inventoryData[type][id].Count == 0)
            {
                _inventoryData[type].Remove(id);
            }if (_inventoryData[type].Count<0)
            {
                _inventoryData.Remove(type);
            }
            Save();
            OnInventoryChanged?.Invoke(type,id,uuid,InventoryItemChangeType.Remove);

        }
        else
        {
            Debug.LogWarning("Item not found in inventory");
        }
    }
    
    public void RemoveAll(string type)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var allItemsByType = _inventoryData[type];
        foreach (var allItems in allItemsByType)
        {
            RemoveAll(type,allItems.Key);
        }

    }
    public void RemoveAll(string type, string id)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var items = _inventoryData[type][id];
        foreach (var item in items)
        {
            Remove(type,id,item.GetId());
        }
    }
    public void RemoveAll(string type, string id, Func<IInventoryItemData,bool> comparer)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return;
        }
        var items = _inventoryData[type][id].FindAll(x=>comparer(x));
        foreach (var item in items)
        {
            Remove(type,id,item.GetId());
        }
        Save();
    }
    public Dictionary<string,Dictionary<string,List<IInventoryItemData>>> Get()
    {
        return _inventoryData;
    }
    public Dictionary<string,List<IInventoryItemData>> Get(string type)
    {
        if (_inventoryData.TryGetValue(type, out var items)) return items;
        Debug.LogWarning("Item not found in inventory");
        return null;
    }
    public List<IInventoryItemData> Get(string type, string id)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return null;
        }
        if (_inventoryData[type].ContainsKey(id)) return _inventoryData[type][id];
        Debug.LogWarning("Item not found in inventory");
        return null;
    }
    

    public List<IInventoryItemData> Get(string type, string id, Func<IInventoryItemData, bool> comparer)
    {
        if (!_inventoryData.ContainsKey(type))
        {
            Debug.LogWarning("Item not found in inventory");
            return null;
        }
        if (!_inventoryData[type].ContainsKey(id))
        {
            Debug.LogWarning("Item not found in inventory");
            return null;
        }
        return _inventoryData[type][id].FindAll(x=>comparer(x));
    }
    public List<string> GetHasInventoryTypes()
    {
        return new List<string>(_inventoryData.Where(x=>x.Value.Count>0).Select(x=>x.Key));
    }
    public List<string> GetHasInventoryItemIds(string type)
    {
        if (_inventoryData.TryGetValue(type, out var value)) return new List<string>(value.Where(x=>x.Value.Count>0).Select(x=>x.Key));
        Debug.LogWarning("Item not found in inventory");
        return null;
    }
    public List<string> GetNoHasInventoryTypes()
    {
        var inventoryTypes = InventoryDataService.GetAllTypes();
        var hasInventoryTypes = new List<string>(_inventoryData.Keys);
        return inventoryTypes.Where(inventoryType => !hasInventoryTypes.Contains(inventoryType)).ToList();
    }
    public List<string> GetNoHasInventoryItemIds(string type)
    {
        if (_inventoryData.TryGetValue(type, out var value))
        {
            var inventoryItemIds = InventoryDataService.GetAllItemIds(type);
            var hasInventoryItemIds = new List<string>(value.Keys);
            return inventoryItemIds.Where(inventoryItemId => !hasInventoryItemIds.Contains(inventoryItemId)).ToList();
        }
        Debug.LogWarning("Item not found in inventory");
        return null;
    }
}

public enum InventoryItemChangeType
{
    Add,
    Update,
    Remove
}



