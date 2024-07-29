using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class ChestSystemEnabler : Editor
{
    static ChestSystemEnabler()
    {
        EditorUtilities.UpdateDefines("ChestManager_Enabled", true);
    }
}