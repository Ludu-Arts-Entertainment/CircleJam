using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class AdSystemEnabler : Editor
{
    static AdSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("AdManager_Enabled",true);
    }
}