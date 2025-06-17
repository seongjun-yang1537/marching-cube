using Corelib.SUI;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;

namespace Ingame
{
    [CustomEditor(typeof(UIComponentBahaviour), true)]
    public class EditorUIComponentBehaviour : OdinEditor
    {
        [NonSerialized]
        UIComponentBahaviour script;
        protected void OnEnable()
        {
            base.OnEnable();
            script = (UIComponentBahaviour)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var childUIsProperty = serializedObject.FindProperty("childUIs");

            SEditorGUILayout.Group("UI Component")
            .Content(
                SEditorGUILayout.Object("Parent", script.parentUI, typeof(UIComponentBahaviour))
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Label("Child UI")
                    + SEditorGUILayout.Property(childUIsProperty)
                )
            )
            .Render();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}