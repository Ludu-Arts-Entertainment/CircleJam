using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class TutorialSystemEnabler : Editor
{
    static TutorialSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("TutorialManager_Enabled",true);
    }
}
