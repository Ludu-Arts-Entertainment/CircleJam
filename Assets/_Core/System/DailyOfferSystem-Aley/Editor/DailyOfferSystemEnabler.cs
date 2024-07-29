using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class DailyOfferSystemEnabler : Editor
{
    static DailyOfferSystemEnabler()
    {
        EditorUtilities.UpdateDefines("DailyOfferManager_Enabled", true);
    }
}
