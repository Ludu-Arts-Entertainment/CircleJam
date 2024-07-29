using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class FriendSystemEnabler : Editor
{
    static FriendSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("FriendManager_Enabled",true);
    }
}
