using System;
using System.Collections.Generic;

public interface IInventoryProvider
{
    IInventoryProvider CreateSelf();
    public event Action<string,string,string,InventoryItemChangeType> OnInventoryChanged;
    void Initialize(Action onReady);
    void Add(string type, string id, IInventoryItemData data);
    void Update(string type, string id, IInventoryItemData data);
    void Increase(string type, string id, string uuid, float amount);
    void Decrease(string type, string id, string uuid, float amount);
    void Remove(string type, string id, string uuid);
    void RemoveAll(string type);
    void RemoveAll(string type, string id);
    void RemoveAll(string type, string id, Func<IInventoryItemData,bool> comparer);
    Dictionary<string, Dictionary<string, List<IInventoryItemData>>> Get();
    Dictionary<string,List<IInventoryItemData>> Get(string type);
    List<IInventoryItemData> Get(string type, string id);
    List<IInventoryItemData> Get(string type, string id, Func<IInventoryItemData,bool> comparer);
    List<string> GetHasInventoryTypes();
    List<string> GetHasInventoryItemIds(string type);
    List<string> GetNoHasInventoryTypes();
    List<string> GetNoHasInventoryItemIds(string type);
}
