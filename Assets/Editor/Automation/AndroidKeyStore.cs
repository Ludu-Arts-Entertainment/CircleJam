using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[InitializeOnLoad]
public class AndroidKeyStore : Editor
{
    static AndroidKeyStore()
    {
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = "Assets/Keys/LuduArtsKeystore.keystore";
        PlayerSettings.Android.keystorePass = "Balina.1500";
        PlayerSettings.Android.keyaliasName = "game";
        PlayerSettings.Android.keyaliasPass = "Balina.1500";
    }
}