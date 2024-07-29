using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GridSystemEnabler : Editor
{
    static GridSystemEnabler()
    {
        EditorUtilities.UpdateDefines("GridManager_Enabled", true);
    }
}