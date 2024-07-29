using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class PoolManagerEnabler : Editor
{
    static PoolManagerEnabler ()
    {
        EditorUtilities.UpdateDefines("PoolManager_Enabled",true);
    }
}

