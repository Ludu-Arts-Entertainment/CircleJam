using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	[InitializeOnLoad]
	public class TimeScalerSlider
	{
		private const float MinValue = 0f;
		private const float MaxValue = 3f;
		private const float Increment = 0.1f;
		static float _sliderValue = 1.0f;
		static TimeScalerSlider()
		{
			ToolbarExtender.RightToolbarGUI.Add(OnTimeScalerGUI);
		}

		static void OnTimeScalerGUI()
		{
			var sliderOptions = new GUILayoutOption[] {GUILayout.Width(200)};
			
			EditorGUILayout.LabelField("TimeScale:", GUILayout.Width(70f));
			
			_sliderValue = EditorGUILayout.Slider(_sliderValue,MinValue, MaxValue, sliderOptions);

			// Add a decrement button to the left of the slider
			if (GUILayout.Button("-", GUILayout.Width(20f)))
			{
				_sliderValue -= Increment;
				_sliderValue = Mathf.Clamp(_sliderValue, MinValue, MaxValue);
			}

			// Add an increment button to the right of the slider
			if (GUILayout.Button(".5", GUILayout.Width(20f)))
			{
				_sliderValue = .5f;
			}
			
			// Add an increment button to the right of the slider
			if (GUILayout.Button("1", GUILayout.Width(20f)))
			{
				_sliderValue = 1;
			}
			
			// Add an increment button to the right of the slider
			if (GUILayout.Button("2", GUILayout.Width(20f)))
			{
				_sliderValue = 2;
			}
			
			// Add an increment button to the right of the slider
			if (GUILayout.Button("+", GUILayout.Width(20f)))
			{
				_sliderValue += Increment;
				_sliderValue = Mathf.Clamp(_sliderValue, MinValue, MaxValue);
			}
			
			Time.timeScale = _sliderValue;
		}
	}
}