using System;
using Cysharp.Threading.Tasks;
using Ludu.RemoteBuild.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Environment = Ludu.RemoteBuild.Data.Environment;

namespace Ludu.RemoteBuild
{
    public class RemoteBuildCenterEditor : EditorWindow
    {
        private static string baseUrl =>
            "https://ludu:110f364cfd1813eb674de21df3c25541a1@firefly-great-piranha.ngrok-free.app/job/";

        private static BuildParameterData _buildParameterData;

        private static string url =>
            baseUrl
            + $"{_buildParameterData.ProjectName}"
            + "/buildWithParameters?"
            + $"token={_buildParameterData.ProjectToken}"
            + $"&BRANCH=origin/{_buildParameterData.Branch}"
            + $"&ENVIRONMENT={_buildParameterData.Environment}"
            + $"&PLATFORM={_buildParameterData.Platform}"
            + $"&VERSION_NUMBER={_buildParameterData.BuildVersion}"
            + $"&BUILD_NUMBER={_buildParameterData.BuildNumber}";

        #region Editor Settings

        private static RemoteBuildCenterEditor _window;
        private static float _spaceWith = 10;

        #endregion

        private static string _result = "-----";

        [MenuItem("Ludu/Build Center")]
        private static void Init()
        {
            _buildParameterData = Resources.Load<BuildParameterData>($"{nameof(BuildParameterData)}")?.Clone();
            if (_buildParameterData == null)
            {
                var paramData = CreateInstance<BuildParameterData>();
                AssetDatabase.CreateAsset(paramData, $"Assets/Resources/{nameof(BuildParameterData)}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _buildParameterData = paramData.Clone();
            }

            _window = GetWindow<RemoteBuildCenterEditor>("Build Center", true, typeof(SceneView));
            _window.Show();
        }

        private void OnGUI()
        {
            _buildParameterData ??= Resources.Load<BuildParameterData>($"{nameof(BuildParameterData)}").Clone();
            GUIStyle style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(10, 10, 10, 10)
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.LabelField("Build Center".ToUpper(), headerStyle);
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.ProjectName =
                EditorGUILayout.TextField("Project Jenkins Name", _buildParameterData.ProjectName);
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.ProjectToken =
                EditorGUILayout.TextField("Project Jenkins Token", _buildParameterData.ProjectToken);
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.Environment =
                (Environment)EditorGUILayout.EnumPopup("Environment", _buildParameterData.Environment);
            EditorGUILayout.Space(_spaceWith);
            EditorGUILayout.BeginHorizontal();
            _buildParameterData.IsCustomBranch =
                EditorGUILayout.Toggle("Custom Branch", _buildParameterData.IsCustomBranch);

            if (_buildParameterData.IsCustomBranch)
            {
                _buildParameterData.Branch = EditorGUILayout.TextField("Branch", _buildParameterData.Branch);
            }
            else
            {
                _buildParameterData.Branch = Enum.TryParse<Branch>(_buildParameterData.Branch, out var branch)
                    ? ((Branch)EditorGUILayout.EnumPopup("Branch", branch)).ToString()
                    : ((Branch)EditorGUILayout.EnumPopup("Branch", Branch.main)).ToString();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.Platform =
                (Platform)EditorGUILayout.EnumPopup("Platform", _buildParameterData.Platform);
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.BuildVersion = EditorGUILayout.TextField("Version", _buildParameterData.BuildVersion);
            EditorGUILayout.Space(_spaceWith);
            _buildParameterData.BuildNumber = EditorGUILayout.IntField("Build Number", _buildParameterData.BuildNumber);
            EditorGUILayout.Space(_spaceWith);
            if (GUILayout.Button("Build", buttonStyle)) Build();

            EditorGUILayout.LabelField(_result, GUILayout.Height(100));
            EditorGUILayout.EndVertical();
        }

        private static async void Build()
        {
            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("ngrok-skip-browser-warning", "true");
            _result = "Build started!";
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                _result += "\n" + request.error;
            }
            else
            {
                _result += "\nStatus: " + request.downloadHandler.isDone;
            }
            var data = Resources.Load<BuildParameterData>($"{nameof(BuildParameterData)}");
            data.ProjectName = _buildParameterData.ProjectName;
            data.ProjectToken = _buildParameterData.ProjectToken;
            data.Environment = _buildParameterData.Environment;
            data.Branch = _buildParameterData.Branch;
            data.Platform = _buildParameterData.Platform;
            data.BuildVersion = _buildParameterData.BuildVersion;
            data.BuildNumber = _buildParameterData.BuildNumber;
            data.IsCustomBranch = _buildParameterData.IsCustomBranch;
            EditorUtility.SetDirty(data);
        }
    }
}