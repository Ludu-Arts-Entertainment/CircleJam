using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryCollection", menuName = "ScriptableObjects/InventoryCollection", order = 1)]
public class InventoryCollection : ScriptableObject
{
    public List<ListOfInventoryItems> InventoryItems = new ();
}

[Serializable]
public record ListOfInventoryItems
{
    [Dropdown("GetSubtypeValues")]
    public string InventoryBlockType;
    public List<InventoryItem> InventoryItems = new List<InventoryItem>();
    private DropdownList<string> GetSubtypeValues()
    {
        var list = new DropdownList<string>();
        foreach (var str in (typeof(InventoryBlockTypes)).GetAllPublicConstantValues<string>())
        {
            list.Add(str,str);
        }
        return list;
    }
}
