using System.IO;
using UnityEditor;
using UnityEngine;

namespace Automation
{
    [CreateAssetMenu(menuName = "Automation/Fastlane Setup PostProcess", fileName = "FastlaneSetupPostProcess", order = 1)]
    public class FastlaneSetupPostProcess : BuildPostProcess
    {
        [SerializeField] private bool _copyExportOptions;
        
        private const string AUTOMATION_PATH = "Editor/Automation/Fastlane";
        private static string AppfileTemplatePath => Path.Combine(Application.dataPath, AUTOMATION_PATH, "Appfile");
        private static string FastfileTemplatePath => Path.Combine(Application.dataPath, AUTOMATION_PATH, "Fastfile");
        private static string ExportOptionsTemplatePath => Path.Combine(Application.dataPath.Replace("/Assets", ""), "ExportOptions.plist");
    
        public override void Execute(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS) 
            {
                Debug.Log($"[{nameof(FastlaneSetupPostProcess)}] Started...");
                
                // Define the paths for the Fastfile and Appfile in the build path
                var fastlaneDirectoryPath = Path.Combine(buildPath, "fastlane");
            
                string appfilePath = Path.Combine(fastlaneDirectoryPath, "Appfile");
                string fastfilePath = Path.Combine(fastlaneDirectoryPath, "Fastfile");

                Directory.CreateDirectory(fastlaneDirectoryPath);
            
                // Copy the Appfile template to the build path
                File.Copy(AppfileTemplatePath, appfilePath, true);
        
                // Copy the Fastfile template to the build path
                File.Copy(FastfileTemplatePath, fastfilePath, true);
            
                // Copy the ExportOptions template to the build path
                if (_copyExportOptions)
                {
                    File.Copy(ExportOptionsTemplatePath, buildPath, true);   
                }
                
                Debug.Log($"[{nameof(FastlaneSetupPostProcess)}] Completed!");
            }
        }
    }
}