using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class DataSystemEnabler : Editor
{
    static DataSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("DataManager_Enabled",true);
    }
}
