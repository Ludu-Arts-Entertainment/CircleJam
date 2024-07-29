using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class EventSystemEnabler : Editor
{
    static EventSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("EventManager_Enabled",true);
    }
}
