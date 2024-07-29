using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private InventoryManager _inventoryManager;
    public InventoryManager InventoryManager => _inventoryManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.InventoryManager] as InventoryManager;
}
