using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Automation
{
    [CreateAssetMenu(menuName = "Automation/Container", fileName = "BuildPostProcessContainer", order = 0)]
    public class BuildPostProcessContainer : ScriptableObject
    {
        [SerializeField] private List<BuildPostProcess> _postProcesses;

        private const string CONTAINER_PATH = "Assets/Editor/Automation/Data/BuildPostProcessContainer.asset";
        
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            Debug.Log($"[{nameof(BuildPostProcessContainer)}] Started...");
            var container = AssetDatabase.LoadAssetAtPath<BuildPostProcessContainer>(CONTAINER_PATH);
            container._postProcesses
                .Where(pp => pp.Enabled)
                .ToList()
                .ForEach(pp => pp.Execute(buildTarget, buildPath));
        }
    }
}