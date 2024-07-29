using UnityEditor;

[InitializeOnLoad]
public class ProductManagementSystemEnabler : Editor
{
    static ProductManagementSystemEnabler ()
    {
        EditorUtilities.UpdateDefines("ProductManager_Enabled",true);
        // EditorUtilities.UpdateDefines("IAPManager_Enabled",true);
    }
}