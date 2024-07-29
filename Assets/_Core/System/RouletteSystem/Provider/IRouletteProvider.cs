using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRouletteProvider
{
    IRouletteProvider CreateSelf();
    void Initialize(Action onReady);
    Dictionary<int, RouletteSaveData> CreateNewRouletteDatas();
}
