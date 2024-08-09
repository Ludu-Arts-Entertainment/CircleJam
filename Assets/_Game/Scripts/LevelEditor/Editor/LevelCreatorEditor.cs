#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelCreator))]
[ExecuteInEditMode]
public class LevelCreatorEditor : Editor
{
    void OnEnable() 
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
 
    void OnDisable() {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView SceneView) 
    {
        var levelCreator = target as LevelCreator;
        
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
 
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            Handles.color = Color.red;
            Handles.DrawLine(hit.point, hit.point + Vector3.up * 10f);
            HandleUtility.Repaint();
        }
        
        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {              
                if (Physics.Raycast(ray, out hit)) 
                {
                    var cell = hit.transform.GetComponent<GridCellEditor>();
                    levelCreator.AddObject(cell);
                }
            }
        }
        if (e.button == 1 ||e.button == 2)
        {
            if (e.type == EventType.MouseDown)
            {              
                if (Physics.Raycast(ray, out hit)) 
                {
                    var cell = hit.transform.GetComponent<GridCellEditor>();
                    cell.RemoveObject();
                }
            }
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            levelCreator.ClearAll();
      
    }
}
#endif