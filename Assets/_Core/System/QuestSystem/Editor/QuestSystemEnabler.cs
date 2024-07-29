using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class QuestSystemEnabler : Editor
{
    static QuestSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("QuestManager_Enabled",true);
    }
}