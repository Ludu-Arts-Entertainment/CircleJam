using UnityEditor;

[InitializeOnLoad]
public class UIManagerEnabler : Editor
{
    static UIManagerEnabler ()
    {
        EditorUtilities.UpdateDefines("UIManager_Enabled",true);
    }
}

