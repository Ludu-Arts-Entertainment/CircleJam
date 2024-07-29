using System.IO;
using UnityEngine;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
#endif
using UnityEditor.Callbacks;


namespace Automation
{
    /// <summary>
    /// Automatically disables Bitcode on iOS builds
    /// </summary>
    public static class DisableAlwaysEmbedSwiftStandardLibraries
    {
        [PostProcessBuildAttribute(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
        {
#if UNITY_IOS
            if (buildTarget != BuildTarget.iOS) return;
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            //Disabling AlwaysEmbedSwiftStandardLibraries on all targets
            //Main
            string target = pbxProject.GetUnityMainTargetGuid();
            //Unity Tests
            //target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            pbxProject.WriteToFile(projectPath);
#endif
        }
    }
}