using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData 
{
    public Dictionary<int, RouletteSaveData> RouletteData = new();
    public DateTime LastRouletteUpdateTime = new DateTime(1970, 1, 1);
}
