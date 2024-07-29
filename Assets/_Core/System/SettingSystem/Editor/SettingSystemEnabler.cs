using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class SettingSystemEnabler : Editor
{
    static SettingSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("SettingManager_Enabled",true);
    }
}

