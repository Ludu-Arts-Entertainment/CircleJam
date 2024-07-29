using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class RouletteSystemEnabler : Editor
{
    static RouletteSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("RouletteManager_Enabled",true);
    }
}
