using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class DailyLoginSystemEnabler : Editor
{
    static DailyLoginSystemEnabler()
    {
        EditorUtilities.UpdateDefines("DailyLoginManager_Enabled", true);
    }
}


