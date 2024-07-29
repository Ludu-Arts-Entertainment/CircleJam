using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class HapticSystemEnabler : Editor
{
    static HapticSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("HapticManager_Enabled",true);
    }
}