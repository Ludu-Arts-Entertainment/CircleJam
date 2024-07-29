using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class InventorySystemEnabler : Editor
{
    static InventorySystemEnabler ()
    {
        EditorUtilities.UpdateDefines("InventoryManager_Enabled",true);
    }
}