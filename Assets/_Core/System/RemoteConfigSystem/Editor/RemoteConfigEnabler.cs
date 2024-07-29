using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class RemoteConfigEnabler : Editor
{
    static RemoteConfigEnabler ()
    {
        EditorUtilities.UpdateDefines("RemoteConfigManager_Enabled",true);
    }
}

