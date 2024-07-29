using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

namespace Automation
{
    [CreateAssetMenu(menuName = "Automation/iOS PostProcess", fileName = "IosProjectPostProcess", order = 0)]
    public class IosProjectPostProcess : BuildPostProcess
    {
        public override void Execute(BuildTarget buildTarget, string pathToBuildProject)
        {
        #if UNITY_IOS
            if (buildTarget != BuildTarget.iOS) return;
            Debug.Log($"[{nameof(IosProjectPostProcess)}] Started...");
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            //Main
            string target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            
            //Unity Tests
            target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");


            pbxProject.WriteToFile(projectPath);
            
            // Get the Info.plist file path
            string plistPath = Path.Combine(pathToBuildProject, "Info.plist");

            // Load the Info.plist file into a PlistDocument
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Add the ITSAppUsesNonExemptEncryption key with value false
            plist.root.SetString("ITSAppUsesNonExemptEncryption", "false");

            // Write the modified Info.plist back to disk
            File.WriteAllText(plistPath, plist.WriteToString());
            
            Debug.Log($"[{nameof(IosProjectPostProcess)}] Completed.");
        #endif
        }
    }
}