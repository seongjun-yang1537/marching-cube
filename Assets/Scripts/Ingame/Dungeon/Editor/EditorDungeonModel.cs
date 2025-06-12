using UnityEngine;
using UnityEditor;
using Corelib.SUI;

namespace Ingame
{
    [CustomEditor(typeof(DungeonModel))]
    public class EditorDungeonModel : Editor
    {
        DungeonModel script;
        void OnEnable()
        {
            script = (DungeonModel)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Button("Teleport Start Room")
                .OnClick(() => script.TeleportPlayerAtStartRoom())
            )
            .Render();
        }
    }
}