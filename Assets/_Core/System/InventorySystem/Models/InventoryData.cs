using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    public Dictionary<string,Dictionary<string,Dictionary<Type,List<string>>>> InventoryItems = new ();
}