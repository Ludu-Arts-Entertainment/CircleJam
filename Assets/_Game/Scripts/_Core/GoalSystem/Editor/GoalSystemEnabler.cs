using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GoalSystemEnabler : Editor
{
    static GoalSystemEnabler()
    {
        EditorUtilities.UpdateDefines("GoalManager_Enabled", true);
    }
}