using UnityEngine;
using UnityEditor;
using Corelib.SUI;

namespace Ingame
{
    [CustomEditor(typeof(DungeonManager))]
    public class EditorDungeonManager : Editor
    {
        DungeonManager script;
        void OnEnable()
        {
            script = (DungeonManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Button("Next Floor")
                .OnClick(() => script.EnterNextFloor())
            )
            .Render();
        }
    }
}