#if !InventoryManager_Modified
using System;
using NaughtyAttributes;

[Serializable]
public record InventoryItem
{
    [Dropdown("GetSubtypeValues")]
    public string Id;
    public string Name;
    private DropdownList<string> GetSubtypeValues()
    {
        var list = new DropdownList<string>();
        foreach (var str in (typeof(InventoryItemTypes)).GetAllPublicConstantValues<string>())
        {
            list.Add(str,str);
        }
        return list;
    }
}
#endif