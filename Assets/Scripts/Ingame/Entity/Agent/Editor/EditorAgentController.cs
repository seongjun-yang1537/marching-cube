using UnityEngine;
using UnityEditor;
using Corelib.SUI;
using Unity.VisualScripting;
using System.Security.Cryptography;

namespace Ingame
{
    [CustomEditor(typeof(AgentController))]
    public class EditorAgentController : Editor
    {
        private Editor moveableEditor;

        AgentController script;
        AgentMoveable moveable;
        void OnEnable()
        {
            script = (AgentController)target;
            moveable = target.GetComponent<AgentMoveable>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Inspecotr_MoveableEditor();
        }

        bool showMoveable = true;
        private void Inspecotr_MoveableEditor()
        {
            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SGUILayout.Horizontal(
                    SGUILayout.Space(15)
                    + SEditorGUILayout.FoldoutHeaderGroup("Moveable Inspector", showMoveable)
                        .OnValueChanged(value => showMoveable = value)
                )
            )
            .Render();

            if (showMoveable)
            {
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Action(() =>
                    {
                        if (moveableEditor == null || moveableEditor.target != moveable)
                        {
                            moveableEditor = CreateEditor(moveable);
                        }

                        if (moveable != null)
                        {
                            moveableEditor.OnInspectorGUI();
                        }
                    })
                )
                .Render();
            }
        }
    }
}