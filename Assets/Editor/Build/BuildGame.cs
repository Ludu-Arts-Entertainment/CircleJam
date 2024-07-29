using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Automation;
using System.IO;

public class BuildHelper
{
    private static string BuildsFolder = "Builds";
    private static string EventDefine = "EVENT_BUILD";
    private static string ProdDefine = "PROD_BUILD";

    private enum Environment
    {
        Dev,
        Main,
        Release
    }

    private static void AddBuildDefines(BuildTargetGroup group, List<string> defines)
    {
        Debug.Log($"[Build] Adding defines: {defines}");
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        List<string> currentDefines = currentSymbols.Split(';').ToList();
        currentDefines.AddRange(defines);
        string updatedSymbols = string.Join(";", currentDefines.ToArray());
        Debug.Log($"[Build] Result Defines: {updatedSymbols}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, updatedSymbols);
        AssetDatabase.Refresh();
    }

    private static void RemoveBuildDefines(BuildTargetGroup group, List<string> defines)
    {
        Debug.Log($"[Build] Removing defines: {defines}");
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        List<string> currentDefines = currentSymbols.Split(';').ToList();
        currentDefines.RemoveAll(d => defines.Contains(d));
        string updatedSymbols = string.Join(";", currentDefines.ToArray());
        Debug.Log($"[Build] Result Defines: {updatedSymbols}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, updatedSymbols);
        AssetDatabase.Refresh();
    }

    [MenuItem("Ludu/Build/Increase Build Version")]
    static void IncreaseBuildVersion()
    {
        PlayerSettings.Android.bundleVersionCode++;
        PlayerSettings.iOS.buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
        EditorApplication.ExecuteMenuItem("File/Save Project");
        AssetDatabase.Refresh();
        Debug.LogWarning($"[Build] Set build version to {PlayerSettings.Android.bundleVersionCode}");
    }

    [MenuItem("Ludu/Build/Build Android Dev")]
    static void BuildAndroidDev()
    {
        PerformAndroidBuild(Environment.Dev, false);
    }

    [MenuItem("Ludu/Build/Build iOS Dev")]
    static void BuildiOSDev()
    {
        PerformiOSBuild(Environment.Dev);
    }

    // [MenuItem("Ludu/Build/Build Android Main")]
    static void BuildAndroidMain()
    {
        PerformAndroidBuild(Environment.Main, false);
    }

    //[MenuItem("Ludu/Build/Build iOS Main")]
    static void BuildiOSMain()
    {
        PerformiOSBuild(Environment.Main);
    }

    //[MenuItem("Ludu/Build/Build Android Production")]
    static void BuildAndroidProd()
    {
        PerformAndroidBuild(Environment.Release, true);
    }

    //[MenuItem("Ludu/Build/Build iOS Production")]
    static void BuildiOSProd()
    {
        PerformiOSBuild(Environment.Release);
    }

    static string EnvironmentName(Environment environment) =>
        environment switch
        {
            Environment.Main => "Main",
            Environment.Release => "Release",
            _ => "Dev",
        };

    static void PerformAndroidBuild(Environment environment, bool appBundle)
    {
        VersionUpdate();
        string env = EnvironmentName(environment);
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.show = false;
        string ext = appBundle ? "aab" : "apk";
        string appName = PlayerSettings.productName.Replace(" ", string.Empty);
        string targetFile = $"{appName}.{ext}";
        targetPath = $"{BuildsFolder}/Android/{env}/{PlayerSettings.Android.bundleVersionCode}/{targetFile}";
        EditorUserBuildSettings.buildAppBundle = appBundle;
        PlayerSettings.Android.useAPKExpansionFiles = environment == Environment.Release;
        PerformBuild(environment, targetPath, BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.Android.useAPKExpansionFiles = false;
    }

    public static string targetPath = "";

    static void PerformiOSBuild(Environment environment)
    {
        VersionUpdate();
        string env = EnvironmentName(environment);
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.SplashScreen.show = false;

        targetPath = $"{BuildsFolder}/iOS/{env}/{PlayerSettings.iOS.buildNumber}";
        PerformBuild(environment, targetPath, BuildTargetGroup.iOS, BuildTarget.iOS);
    }

    public static void VersionUpdate()
    {
        string filePath = Path.Combine(Application.dataPath, "version.txt");
        Debug.Log("version update path: "+ filePath);
        if (File.Exists(filePath))
        {
            string version = File.ReadAllText(filePath).Trim();
            var t = version.Split("-");
            Debug.Log("File Exist and i write in settings");

            PlayerSettings.bundleVersion = t[0];
            PlayerSettings.iOS.buildNumber = t[1];
            PlayerSettings.Android.bundleVersionCode = int.Parse(t[1]);
        }
        else
        {
            PlayerSettings.bundleVersion = "0.1";
            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.Android.bundleVersionCode = 1;
            Debug.Log("Default value set");
        }
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    static void PerformBuild(Environment environment, string BuildsFolder, BuildTargetGroup targetGroup,
        BuildTarget buildTarget)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

        BuildOptions buildOptions = BuildOptions.None;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnabledEditorScenes();
        buildPlayerOptions.locationPathName = BuildsFolder;
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.options = buildOptions;

        var eventDefinesList = new List<string> { EventDefine, ProdDefine };
        var prodDefinesList = new List<string> { ProdDefine };

        switch (environment)
        {
            case Environment.Dev:
                RemoveBuildDefines(targetGroup, eventDefinesList);
                break;
            case Environment.Main:
                AddBuildDefines(targetGroup, eventDefinesList);
                break;
            case Environment.Release:
                AddBuildDefines(targetGroup, prodDefinesList);
                break;
        }

        try
        {
            Debug.Log($"[Build] Started");
            BuildReport res = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildPostProcessContainer.OnPostProcessBuild(buildTarget, targetPath);
            Debug.Log($"[Build] Finished");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Build] failed: {e}");
        }

        EditorUserBuildSettings.buildAppBundle = false;
        if (environment != Environment.Dev)
        {
            RemoveBuildDefines(targetGroup, eventDefinesList);
        }

        EditorApplication.ExecuteMenuItem("File/Save Project");
    }
}