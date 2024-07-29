using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

public class JSONtoSOEditor : EditorWindow
{
    private static JSONtoSOEditor _window;
    private string _jsonPath = "ExternalPackages";
    private string _targetFolder = "ExternalPackages";
    private bool _isMultiple;
    [MenuItem("Ludu/Json to SO")]
    private static void Init()
    {
        _window = GetWindow<JSONtoSOEditor>("Json To ScriptableObject", true, typeof(SceneView));
        _window.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(500));
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        _jsonPath = EditorGUILayout.TextField("Import File", _jsonPath, GUILayout.Width(400));
        if (GUILayout.Button("Open")) 
            _jsonPath = EditorUtility.OpenFilePanel("Select Json", "", ".json");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        _targetFolder = EditorGUILayout.TextField("Target Folder", _targetFolder, GUILayout.Width(400));
        if (GUILayout.Button("Open")) 
            _targetFolder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        _isMultiple = EditorGUILayout.Toggle("Is Multiple", _isMultiple,GUILayout.Width(50));
        EditorGUILayout.Space(20);
        if (_filteredTypes is { Length: 0 } or null)
        {
            _filteredTypes = InitializeType<ICSV>();
            _filteredTypeNames = _filteredTypes.Select(t => t.ReflectedType == null ? t.Name : $"t.ReflectedType.Name + t.Name")
                .ToArray();
        }
        var selectedIndex = EditorGUILayout.Popup(_selectedTypeIndex, _filteredTypeNames);
        if (selectedIndex != _selectedTypeIndex)
        {
            _selectedTypeIndex = selectedIndex;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20);
        if (GUILayout.Button("Convert", GUILayout.Height(30)))
        {
            if (_isMultiple)
            {
                ConvertMultiple();
            }
            else
            {
                Convert();
            }
        }
        EditorGUILayout.Space(20);
        EditorGUILayout.EndVertical();
    }
    
    private void Convert()
    {
        var projectPath = Application.dataPath;
        var json = File.ReadAllText(_jsonPath);
        var fromJson = (ScriptableObject)JsonHelper.FromJson(json,_selectedType);
        var fileName = Path.GetFileName(_jsonPath);
        var path = $"{_targetFolder}/{fileName}.asset";
        AssetDatabase.CreateAsset(fromJson, path.Replace(projectPath,"Assets"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void ConvertMultiple()
    {
        var projectPath = Application.dataPath;
        var jsonFile = File.ReadAllText(_jsonPath);
        Type genericListType = typeof(List<>).MakeGenericType(_selectedType);
        var jsonList = (IList)JsonHelper.FromJson(jsonFile, genericListType);
        foreach (var json in jsonList)
        {
            var fileName = ((IJson)json).SOName;
            var path = $"{_targetFolder}/{fileName}.asset";
            AssetDatabase.CreateAsset((ScriptableObject)json, path.Replace(projectPath,"Assets"));
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private TypeFilterAttribute typeFilter;
    private Type[] _filteredTypes;
    private string[] _filteredTypeNames;
    private int _selectedTypeIndex;
    private Type _selectedType=> _filteredTypes[_selectedTypeIndex];
    Type[] InitializeType<T>()
    {
        typeFilter = new TypeFilterAttribute(typeof(T));
        var filteredTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => typeFilter == null ? DefaultFilter(t) :  typeFilter.Filter(t))
            .ToArray();
        return filteredTypes;
    }
    static bool DefaultFilter(Type type)
    {
        return !type.IsAbstract && !type.IsInterface && !type.IsGenericType;
    }
}

