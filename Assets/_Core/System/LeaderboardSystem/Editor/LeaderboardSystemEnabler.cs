using UnityEditor;

[InitializeOnLoad]
public class LeaderboardSystemEnabler : Editor
{
    static LeaderboardSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("LeaderboardManager_Enabled",true);
    }
}