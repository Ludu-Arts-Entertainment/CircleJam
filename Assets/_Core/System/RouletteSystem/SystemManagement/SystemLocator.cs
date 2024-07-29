using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SystemLocator
{
    private RouletteManager _rouletteManager;
    public RouletteManager RouletteManager =>
        _rouletteManager ??= GameInstaller.Instance.ManagerDictionary[ManagerEnums.RouletteManager] as RouletteManager;
}