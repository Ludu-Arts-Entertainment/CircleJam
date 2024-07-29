using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class LevelSystemEnabler : Editor
{
    static LevelSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("LevelManager_Enabled",true);
    }
}
