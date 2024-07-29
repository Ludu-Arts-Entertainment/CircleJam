using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDataService
{
    private static List<ListOfInventoryItems> _listOfInventoryItems = new List<ListOfInventoryItems>();
    public static bool isInitialized = false;
    
    public static void Initialize()
    {
        _listOfInventoryItems = Resources.Load<InventoryCollection>("InventoryCollection").InventoryItems;
        isInitialized = true;
    }
    public static InventoryItem Get(string type, string id)
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"{nameof(InventoryDataService)} is not initialized");
            Initialize();
        }
        var item = _listOfInventoryItems?.Find(x => x.InventoryBlockType == type).InventoryItems?.Find(x => x.Id == id);
        if (item != null) return item;
        Debug.LogError($"Item with id {id} not found in inventory");
        return null;
    }
    public static List<string> GetAllTypes()
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"{nameof(InventoryDataService)} is not initialized");

            Initialize();
        }
        var types = new List<string>();
        foreach (var inventoryItem in _listOfInventoryItems)
        {
            types.Add(inventoryItem.InventoryBlockType);
        }
        return types;
    }
    public static List<string> GetAllItemIds(string type)
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"{nameof(InventoryDataService)} is not initialized");

            Initialize();
        }
        var ids = new List<string>();
        foreach (var inventoryItem in _listOfInventoryItems.Find(x => x.InventoryBlockType == type).InventoryItems)
        {
            ids.Add(inventoryItem.Id);
        }
        return ids;
    }
}
