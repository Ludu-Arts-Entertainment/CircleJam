using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
public class ExchangeSystemEnabler : Editor
{
    static ExchangeSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("ExchangeManager_Enabled",true);
    }
}
