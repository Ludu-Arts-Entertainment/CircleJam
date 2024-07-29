using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class AudioSystemEnabler : Editor
{
    static AudioSystemEnabler()
    {
        EditorUtilities.UpdateDefines("AudioManager_Enabled", true);
    }
}