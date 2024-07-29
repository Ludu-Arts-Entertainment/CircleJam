using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestContainer", menuName = "ScriptableObjects/ChestContainer", order = 1)]
public class ChestContainer : ScriptableObject
{
    public List<ChestData> ChestDataList;
}
