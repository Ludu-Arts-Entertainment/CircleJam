using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalConfigData", menuName = "ScriptableObjects/Config/LocalConfigData", order = 1)]
public class LocalConfigData : ScriptableObject
{
    [SubclassSelector,SerializeReference]
    public List<IConfig> ListOfConfigs;
}

