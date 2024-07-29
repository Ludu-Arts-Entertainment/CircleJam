using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Ludu.PackagesManager.Editor
{
    public class LuduPackagesManagerEditor : EditorWindow
    {
        private static LuduPackagesManagerEditor _window;
        private static string _luduPackagesJson;
        Packages _packages;
        
        private string _packageFolder = "ExternalPackages";
        private string _packageResult;
        
        [MenuItem("Ludu/Package Downloader")]
        private static void Init()
        {
            _window = GetWindow<LuduPackagesManagerEditor>("Ludu Packages", true, typeof(SceneView));
            _window.Show();

            var path = Directory.GetParent(Directory.GetParent(new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName()).FullName).FullName;
            var packagesPath = $"{path}/package.json";

            _luduPackagesJson = File.ReadAllText(packagesPath);
        }
        
        private void OnGUI()
        {
            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));

            UseFireBase();

            EditorGUILayout.EndVertical();
        }
        
        private void UseFireBase()
        {
            var path = Directory.GetParent(Directory.GetParent(new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName()).FullName).FullName;
            var packagesPath = $"{path}/package.json";
            if (_packages == null || _packages.packages.Count==0)
            {
                
                _luduPackagesJson = File.ReadAllText(packagesPath);
                if (_luduPackagesJson != null)
                {
                    _packages = JsonUtility.FromJson<Packages>(_luduPackagesJson);
                }
            }

            if (_packages != null)
            {

                EditorGUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Instructions"))
                    Application.OpenURL("https://developers.google.com/unity/instructions");
                if (GUILayout.Button("Archive"))
                    Application.OpenURL("https://developers.google.com/unity/archive");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(20);

                _packageFolder = EditorGUILayout.TextField("Download Folder", _packageFolder);
                EditorGUILayout.Space(20);

                EditorGUILayout.BeginHorizontal(GUILayout.Width(400));

                EditorGUILayout.BeginVertical(GUILayout.Width(200));
                _packages.packages.ForEach(s =>
                {
                    if (s.id != "ExternalDependencyManager" && s.id != "FirebaseApp")
                    {
                        s.selected = EditorGUILayout.ToggleLeft(s.id, s.selected);
                    }
                });
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.Width(400));
                GUILayout.Space(20);
                
                EditorGUILayout.LabelField("Add to \"Editor Coroutine\" Package from Package Manager");
                GUILayout.Space(20);
                
                if (GUILayout.Button("DOWNLOAD Selected Packages"))
                {
                    DownloadPackages();
                }
                
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                _packageResult = EditorGUILayout.TextArea(_packageResult, GUILayout.Width(800), GUILayout.Height(200));
            }
        }
        
        private void DownloadPackages()
        {
            List<string> names = new List<string>();
            _packages.packages.ForEach(s =>
            {
                if (s.selected)
                {
                    names.Add(s.id);
                    names.AddRange(s.requireIds);
                }
            });

            names = names.Distinct().ToList();

            List<Package> downloadList = new List<Package>();

            names.ForEach(s =>
            {
                var p = _packages.packages.Find(a => a.id == s);
                if (p != null)
                    downloadList.Add(p);
            });

            EditorCoroutineUtility.StartCoroutine(DownLoadProcess(downloadList), this);
        }
        
        private IEnumerator DownLoadProcess(List<Package> packages)
        {
            string folderPath = Application.dataPath.Replace("Assets", _packageFolder);

            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                _packageResult = "Start Downloading...\n";
                var formattedManifest = JsonHelper.FromJson<ManifestTempClass>(File.ReadAllText("Packages/manifest.json"));
                
                _packageResult+= "\"com.unity.editorcoroutines\": \"1.0.0\",";
                formattedManifest.dependencies.TryAdd("com.unity.editorcoroutines", "1.0.0");
                
                foreach (var package in packages)
                {
                    var urlExt = package.url[^4..];
                    if (urlExt==".git")
                    {
                        _packageResult += "\n\"" + package.name + "\": \""+ package.url + "\",";
                        formattedManifest.dependencies.TryAdd(package.name, package.url);
                        continue;
                    }
                    using (UnityWebRequest uwr = new UnityWebRequest(package.url))
                    {
                        uwr.method = UnityWebRequest.kHttpVerbGET;

                        var tmp = Path.Combine(folderPath, package.name + "-" + package.version + ".tgz");

                        _packageResult += "\n\"" + package.name + "\": \"file:../" + _packageFolder + "/" +
                                          package.name + "-" + package.version + ".tgz\",";

                        formattedManifest.dependencies.TryAdd(package.name, "file:../" + _packageFolder + "/" +  package.name + "-" + package.version + ".tgz");
                        
                        DownloadHandlerFile dh = new DownloadHandlerFile(tmp);

                        dh.removeFileOnAbort = true;

                        uwr.downloadHandler = dh;
                        yield return uwr.SendWebRequest();
                    }
                }
                foreach (var defineSymbol in packages.SelectMany(package => package.symbol))
                {
#if UNITY_EDITOR
                    EditorUtilities.UpdateDefines(defineSymbol, true);
#endif
                }
                
                _packageResult += "\nFinished\n";
                _packageResult = _packageResult.Substring(0, _packageResult.Length - 1);
                formattedManifest.scopedRegistries??= Array.Empty<ScopeRegistries>();
                File.WriteAllText("Packages/manifest.json", JsonHelper.ToJson(formattedManifest));
                //UnityWebRequest uwr = UnityWebRequest.Get(packages[0].url);

            }
        }
    }
    
    [Serializable]
    public class ManifestTempClass
    {
        public Dictionary<string,string> dependencies;
        public ScopeRegistries[] scopedRegistries;
    }
    [Serializable]
    public class ScopeRegistries
    {
        public string name;
        public string url;
        public string[] scopes;
    }

    [Serializable]
    public class Packages
    {
        public List<Package> packages;
    }

    [Serializable]
    public class Package
    {
        public string id;
        public string name;
        public string url;
        public string version;
        public List<string> requireIds;
        public string[] symbol;
        public bool selected;
    }
}