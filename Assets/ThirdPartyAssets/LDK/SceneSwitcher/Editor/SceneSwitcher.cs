using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;
		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 12,
				fixedWidth = 100,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static string _currentSceneName;
		static SceneSwitchLeftButton()
		{
			_currentSceneName = SceneManager.GetActiveScene().name;
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();
			
			if (EditorGUILayout.DropdownButton( new GUIContent($"{_currentSceneName}"), FocusType.Passive, ToolbarStyles.commandButtonStyle))
			{
				// Debug.LogWarning("Scene count: " + EditorSceneManager.sceneCountInBuildSettings);
				GenericMenu menu = new GenericMenu();
				
				for (int index = 0; index < EditorSceneManager.sceneCountInBuildSettings; index++)
				{
					var scenePath = SceneUtility.GetScenePathByBuildIndex(index);
					var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
					menu.AddItem(new GUIContent($"{sceneName}"), false, () => SceneHelper.StartScene($"{sceneName}",ref _currentSceneName));
				}
				
				menu.ShowAsContext();
			}
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName, ref string currentSceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			currentSceneName = sceneName;
			
			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					//EditorApplication.isPlaying = true;
				}
			}
			sceneToOpen = null;
		}
	}
}