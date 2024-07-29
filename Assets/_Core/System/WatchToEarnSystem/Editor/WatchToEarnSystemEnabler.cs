using UnityEditor;

[InitializeOnLoad]
public class WatchToEarnSystemEnabler
{
    static WatchToEarnSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("WatchToEarnManager_Enabled",true);
    }
}
