using UnityEditor;
using UnityEngine;
[InitializeOnLoad]

public class MailSystemEnabler : MonoBehaviour
{
    static MailSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("MailManager_Enabled",true);
    }
}
