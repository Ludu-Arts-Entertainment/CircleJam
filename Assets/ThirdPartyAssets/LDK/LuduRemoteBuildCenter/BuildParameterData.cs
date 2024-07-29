using UnityEngine;

namespace Ludu.RemoteBuild.Data
{
    [CreateAssetMenu(fileName = "BuildParameterData", menuName = "Ludu/Build/BuildParameterData")]
    public class BuildParameterData : ScriptableObject
    {
        public string ProjectName;
        public string ProjectToken;
        public Environment Environment;
        public string Branch;
        public Platform Platform;
        public string BuildVersion;
        public int BuildNumber;
        public bool IsCustomBranch;

        public BuildParameterData Clone()
        {
            return new BuildParameterData
            {
                ProjectName = ProjectName,
                ProjectToken = ProjectToken,
                Environment = Environment,
                Branch = Branch,
                Platform = Platform,
                BuildVersion = BuildVersion,
                BuildNumber = BuildNumber,
                IsCustomBranch = IsCustomBranch
            };
        }
    }
    public enum Environment
    {
        Dev,
        Main,
        Release
    }

    public enum Branch
    {
        dev,
        main,
        release
    }

    public enum Platform
    {
        iOS,
        Android
    }
}