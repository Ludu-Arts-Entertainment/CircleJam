using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class AnalyticsSystemEnabler : Editor
{
    static AnalyticsSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("AnalyticsManager_Enabled", true);
    }
}