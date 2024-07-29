#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;

public class EditorUtilities
{
    public static void UpdateDefines(string entry, bool enabled, BuildTargetGroup[] groups = null)
    {
        groups ??= new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
        foreach (var group in groups)
        {
            var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            var edited = false;
            if (enabled && !defines.Contains(entry))
            {
                defines.Add(entry);
                edited = true;
            }
            else if (!enabled && defines.Contains(entry))
            {
                defines.Remove(entry);
                edited = true;
            }

            if (edited)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
            }
        }
    }
}
#endif