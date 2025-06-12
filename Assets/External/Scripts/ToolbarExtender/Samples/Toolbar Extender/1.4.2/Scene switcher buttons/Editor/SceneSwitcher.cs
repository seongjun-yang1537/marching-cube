using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using Corelib.SUI;
using UnityEditor.UIElements;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Button")
			{
				fontSize = 12,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold,
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			SGUILayout.Horizontal(
				SGUILayout.FlexibleSpace()
				+ SGUILayout.Button("Ingame")
				.OnClick(() => SceneHelper.StartScene("Ingame"))
				.GUIStyle(ToolbarStyles.commandButtonStyle)
				+ SGUILayout.Button("Outgame")
				.OnClick(() => SceneHelper.StartScene("Outgame"))
				.GUIStyle(ToolbarStyles.commandButtonStyle)
				+ SGUILayout.Button("Testbed")
				.OnClick(() => SceneHelper.StartScene("Testbed"))
				.GUIStyle(ToolbarStyles.commandButtonStyle)
			)
			.Render();
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

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

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					// EditorApplication.isPlaying = true;
				}
			}
			sceneToOpen = null;
		}
	}
}