using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChestProvider
{
    IChestProvider CreateSelf();
    void Initialize(System.Action onReady);
    ChestData GetChestData(string chestType);
    List<ProductBlock> Open(string chestType);
}
