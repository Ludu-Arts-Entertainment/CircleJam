using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class EnergySystemEnabler : Editor
{
    static EnergySystemEnabler ()
    {
        EditorUtilities.UpdateDefines("EnergyManager_Enabled",true);
    }
}


