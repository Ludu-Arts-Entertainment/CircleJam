using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class SpecialOfferSystemEnabler : Editor
{
    static SpecialOfferSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("SpecialOfferManager_Enabled",true);
    }
}

