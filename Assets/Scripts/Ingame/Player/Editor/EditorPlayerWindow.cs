using Corelib.SUI;
using UnityEditor;
using UnityEngine;

namespace Ingame
{
    public class EditorPlayerWindow : EditorWindow
    {
        [MenuItem("Tools/Player Window")]
        public static void ShowWindow()
        {
            GetWindow<EditorPlayerWindow>("Player Window");
        }

        private PlayerModel _playerModel;
        protected PlayerModel PlayerModel { get => _playerModel ??= FindAnyObjectByType<PlayerModel>(); }

        private BagGUIDrawer drawer;

        private void OnEnable()
        {
            if (PlayerModel != null)
                drawer = new(PlayerModel.inventory);
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }

        private void OnPlayModeStateChange(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    _playerModel = null;
                    if (PlayerModel == null) return;
                    drawer = new(PlayerModel.inventory);
                    break;
            }
            Repaint();
        }

        private void OnGUI()
        {
            SEditorGUILayout.Vertical()
            .Content(
                SEditorGUILayout.Object("Player Model", PlayerModel, typeof(PlayerModel))
                + SEditorGUILayout.Group("Inventory")
                .Content(
                    SEditorGUILayout.Action(() => drawer?.OnInspectorGUI())
                )
            )
            .Render();
        }
    }
}