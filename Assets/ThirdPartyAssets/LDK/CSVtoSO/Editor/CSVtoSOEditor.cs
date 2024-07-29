using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSVtoSOEditor : EditorWindow
{
    private static CSVtoSOEditor _window;
    private string _csvPath = "ExternalPackages";
    private string _targetFolder = "ExternalPackages";
    
    private ScriptableObject so;
    
    [MenuItem("Ludu/Csv to SO")]
    private static void Init()
    {
        _window = GetWindow<CSVtoSOEditor>("CSV To ScriptableObject", true, typeof(SceneView));
        _window.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        _csvPath = EditorGUILayout.TextField("Import File", _csvPath, GUILayout.Width(500));
        if (GUILayout.Button("Open")) 
            _csvPath = EditorUtility.OpenFilePanel("Select CSV", "", ".csv");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        _targetFolder = EditorGUILayout.TextField("Target Folder", _targetFolder, GUILayout.Width(500));
        if (GUILayout.Button("Open")) 
            _targetFolder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        EditorGUILayout.EndHorizontal();
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
        if (GUILayout.Button("Convert")) Convert();
        EditorGUILayout.Space(20);
        EditorGUILayout.EndVertical();
    }
    
    private void Convert()
    {
        var projectPath = Application.dataPath;
        var csv = File.ReadAllText(_csvPath);
        var lines = csv.Split('\n');
        foreach (var line in lines)
        {
            var values = line.Split(',');
            var obj = ScriptableObject.CreateInstance(_selectedType);
            var fields = _selectedType.GetFields();
            for (var i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.FieldType == typeof(int))
                {
                    field.SetValue(obj, int.Parse(values[i+1]));
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(obj, float.Parse(values[i+1]));
                }
                else if (field.FieldType == typeof(string))
                {
                    field.SetValue(obj, values[i+1]);
                }
                else if (field.FieldType == typeof(List<>))
                {
                    field.SetValue(obj, values[i+1].Split(",").ToList());
                }
            }
            var path = $"{_targetFolder}/{values[0]}.asset";
            AssetDatabase.CreateAsset(obj, path.Replace(projectPath,"Assets"));
            AssetDatabase.SaveAssets();
        }
        
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

