using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class InputSystemEnabler : Editor
{
    static InputSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("InputManager_Enabled",true);
    }
}

