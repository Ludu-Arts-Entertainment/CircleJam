using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IPHONE
using System.IO;
using UnityEditor.iOS.Xcode;
#endif

//Used to add associated domains capability to the Xcode project
//Can also be added manually in XCODE
public class AppleBuildPostProcessor : MonoBehaviour
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }
//TODO: Add your associated domains here
#if UNITY_IPHONE
        //Get the Xcode project
        var projectPath = PBXProject.GetPBXProjectPath(path);
        var project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projectPath));

        var manager = new ProjectCapabilityManager(
            projectPath,
            "Entitlements.entitlements",
            null,
            project.GetUnityMainTargetGuid()
        );
        manager.AddAssociatedDomains(new string[]
        {
            "applinks:www.luduarts.com/ros", 
            "applinks:www.luduarts.com/ros/",
            "applinks:luduarts.com/ros",
            "applinks:luduarts.com/ros/",
        });
        manager.WriteToFile();
#endif
    }
}