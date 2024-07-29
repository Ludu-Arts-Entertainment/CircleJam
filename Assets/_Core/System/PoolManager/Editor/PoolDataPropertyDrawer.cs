using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PoolData))]
public class PoolDataPropertyDrawer : PropertyDrawer
{
    private struct Data
    {
        public SerializedProperty property;
        public bool _hasReference;
    }

    private const string KeyFieldName = nameof(PoolData.Id);
    private const string ObjectFieldName = nameof(PoolData.Prefab);
    private const string InitialCount = nameof(PoolData.InitialCount);
    private const string TagFieldName = nameof(PoolData.Tag);

    SerializedProperty keyProperty;
    SerializedProperty objectProperty;
    SerializedProperty initialCountProperty;
    SerializedProperty tagProperty;

    private string _name;
    private bool _hasReference;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _name = property.displayName;

        keyProperty = property.FindPropertyRelative(KeyFieldName);
        objectProperty = property.FindPropertyRelative(ObjectFieldName);
        initialCountProperty = property.FindPropertyRelative(InitialCount);
        tagProperty = property.FindPropertyRelative(TagFieldName);

        var pos = position.position;
        var width = EditorGUIUtility.currentViewWidth - 80;

        var labelRect = new Rect(pos.x, pos.y, width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, new GUIContent(_name), true);
        position = labelRect;
        if (!property.isExpanded) return;

        pos.x += 15;
        pos.y += EditorGUIUtility.singleLineHeight;
        var keyRect = new Rect(pos.x, pos.y, width, EditorGUIUtility.singleLineHeight);
        pos.y += EditorGUIUtility.singleLineHeight;
        var initialCountRect = new Rect(pos.x, pos.y, width, EditorGUIUtility.singleLineHeight);
        pos.y += EditorGUIUtility.singleLineHeight;
        var tagRect = new Rect(pos.x, pos.y, width, EditorGUIUtility.singleLineHeight);
        pos.y += EditorGUIUtility.singleLineHeight;
        var objectRect = new Rect(pos.x, pos.y, width, EditorGUIUtility.singleLineHeight);
        
        
        var selectButtonRect = new Rect(width + 35, pos.y, 20, EditorGUIUtility.singleLineHeight);

        pos.y += EditorGUIUtility.singleLineHeight;

        var dropdownRect = new Rect(pos.x + 50, pos.y, width - 50, EditorGUIUtility.singleLineHeight);


        // EditorGUI.LabelField(labelRect, new GUIContent(name));
        // EditorGUI.DropdownButton(labelRect, new GUIContent(name), FocusType.Passive);

        var labelWidthRelative = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 50;
        EditorGUI.PropertyField(keyRect, keyProperty, new GUIContent("PrefabId"));
        EditorGUI.PropertyField(tagRect, tagProperty, new GUIContent("Tag"));
        EditorGUI.PropertyField(initialCountRect, initialCountProperty, new GUIContent("Count"));
        EditorGUI.PropertyField(objectRect, objectProperty, new GUIContent("Component"));

        if (objectProperty.objectReferenceValue == null)
        {
        }
        else
        {
            var objects = new List<Object>();

            var referenceObject = objectProperty.objectReferenceValue;

            var isRefenenceValid = false;

            if (referenceObject is Component component)
            {
                isRefenenceValid = true;
                objects.Add(component.gameObject);
                var components = component.GetComponents<Component>();
                objects.AddRange(components);
            }

            // var index = testProperty.intValue;

            if (isRefenenceValid)
            {
                var index = objects.IndexOf(referenceObject);
                var displayedOptions = objects.Select(o => o.GetType().ToString()).ToArray();
                index = EditorGUI.Popup(dropdownRect, index, displayedOptions);

                var newReferenceObject = objects[index];
                // EditorGUI.BeginChangeCheck();
                objectProperty.objectReferenceValue = newReferenceObject;
                // if (EditorGUI.EndChangeCheck())
                // {
                //     property.serializedObject.ApplyModifiedProperties();
                // }
            }
            else
            {
                Debug.LogError("This Object cannot be pooled!");
                objectProperty.objectReferenceValue = null;
            }
        }

        EditorGUIUtility.labelWidth = labelWidthRelative;
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        _hasReference = objectProperty.objectReferenceValue;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var expandedHeight = 0f;
        if (property.isExpanded && property.serializedObject != null)
        {
            var dropdownHeight = !_hasReference ? 0 : EditorGUIUtility.singleLineHeight;
            expandedHeight += dropdownHeight;
        }

        return EditorGUI.GetPropertyHeight(property, true) + expandedHeight;
    }
}