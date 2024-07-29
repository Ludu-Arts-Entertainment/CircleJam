using UnityEditor;

[InitializeOnLoad]
public class MovementSystemEnabler : Editor
{
    static MovementSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("MovementManager_Enabled",true);
    }
}