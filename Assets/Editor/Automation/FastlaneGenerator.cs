using UnityEngine;
using UnityEditor;
using System.IO;
using JetBrains.Annotations;
using UnityEngine.Serialization;

public class FastlaneGeneratorEditor : EditorWindow
{
    private const string FastlaneTemplatePath = "Assets/Editor/Automation/Template/Fastfile_Temp";
    private const string OutputPath = "Assets/Editor/Automation/Fastlane/";
    private const string FastlaneFileName = "Fastfile";

    private string _appfileTemplatePath = "Assets/Editor/Automation/Template/Appfile_Temp";
    private string _appfilePath = "Assets/Editor/Automation/Fastlane/";
    private string _appfileFileName = "Appfile";

    private string _dist = "dist";
    [CanBeNull] private string _signIdentity;
    private string _teamID;
    private string _profileUuıd;
    private string _authKeyId;
    private string _issuerId;
    private string _authPath;
    private string _bundle;

    private static readonly string[] Distributor = new string[]
    {
        "LUDU",
        "DGB"
    };

    private int _selectDistributor;

    private void OnGUI()
    {
        GUILayout.Label("Fastlane Fastfile Generator", EditorStyles.boldLabel);
        _dist = EditorGUILayout.TextField("Dist", _dist);
        _profileUuıd = EditorGUILayout.TextField("Profile UUID", _profileUuıd);
        _bundle = EditorGUILayout.TextField("Bundle", _bundle);

        _selectDistributor = EditorGUILayout.Popup("Select Distrubutor", _selectDistributor, Distributor);

        if (Distributor[_selectDistributor] == "LUDU")
        {
            _signIdentity = "Apple Distribution: LUDU ARTS TEKNOLOJI ANONIM SIRKETI (K2J97CSWZP)";
            _teamID = "K2J97CSWZP";
            _authKeyId = "BZU9U428V6";
            _issuerId = "84c971ba-af7c-4687-ab45-0ac5a979ed33";
            _authPath = "AuthKey_BZU9U428V6.p8";
        }
        else if (Distributor[_selectDistributor] == "DGB")
        {
            _signIdentity = "Apple Distribution: dila gizem bolukbasi (F65B4BVAKH)";
            _teamID = "F65B4BVAKH";
            _authKeyId = "P8CA9FXFHK";
            _issuerId = "a1c84dce-014c-488d-92f7-5401021147ff";
            _authPath = "AuthKey_P8CA9FXFHK.p8";
        }


        if (GUILayout.Button("Generate Fastfile"))
        {
            GenerateFastfile();
        }
    }

    private void GenerateFastfile()
    {
        string fastlaneTemplate = File.ReadAllText(FastlaneTemplatePath);
        string appfileTemplate = File.ReadAllText(_appfileTemplatePath);
        string replacedFastlane = fastlaneTemplate
            .Replace("DIST", _dist)
            .Replace("SIGNIDENTITY", _signIdentity)
            .Replace("TEAMID", _teamID)
            .Replace("PROFILEUUID", _profileUuıd)
            .Replace("KEYID", _authKeyId)
            .Replace("ISSUER", _issuerId)
            .Replace("AUTHPATH", _authPath)
            .Replace("BUNDLE", _bundle);

        string replaced = appfileTemplate
            .Replace("BUNDLE", _bundle)
            .Replace("TEAMID", _teamID);

        string outputFile = Path.Combine(OutputPath, FastlaneFileName);
        string appfile = Path.Combine(_appfilePath, _appfileFileName);
        File.WriteAllText(outputFile, replacedFastlane);
        File.WriteAllText(appfile, replaced);

        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Tools/Fastlane Fastfile Generator")]
    public static void ShowWindow()
    {
        GetWindow<FastlaneGeneratorEditor>("Fastlane Fastfile Generator");
    }
}