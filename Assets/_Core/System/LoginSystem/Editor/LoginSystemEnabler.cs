using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class LoginSystemEnabler : Editor
{
    static LoginSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("LoginManager_Enabled", true);
    }
    
}
