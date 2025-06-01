using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Corelib.SUI;

namespace MCube
{
    [CustomEditor(typeof(ScalarFieldModel))]
    public class EditorScalarFieldModel : Editor
    {
        private Editor scalarFieldEditor;

        ScalarFieldModel script;

        void OnEnable()
        {
            script = (ScalarFieldModel)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();
            Inspector_ScalarFieldScriptableObject();
            Inspector_Option();

            if (GUILayout.Button("Marching Cube"))
            {
                script.GenerateMarchingCube();
            }

            serializedObject.ApplyModifiedProperties();
        }

        bool showScalarFieldScriptableObject;
        private void Inspector_ScalarFieldScriptableObject()
        {
            if (!script.scalarField)
                return;

            { EditorGUILayout.BeginVertical("helpbox"); }
            {
                { GUILayout.BeginHorizontal(); GUILayout.Space(15); }
                {
                    showScalarFieldScriptableObject = EditorGUILayout.BeginFoldoutHeaderGroup(
                                    showScalarFieldScriptableObject, "Scalar Field Inspector");
                }
                { GUILayout.EndHorizontal(); }
            }
            { EditorGUILayout.EndVertical(); }

            if (showScalarFieldScriptableObject)
            {
                { EditorGUILayout.BeginVertical("helpbox"); }
                {
                    if (scalarFieldEditor == null || scalarFieldEditor.target != script.scalarField)
                    {
                        scalarFieldEditor = CreateEditor(script.scalarField);
                    }

                    scalarFieldEditor.OnInspectorGUI();
                }
                { EditorGUILayout.EndVertical(); }
            }

            { EditorGUILayout.EndFoldoutHeaderGroup(); }
        }

        private void Inspector_Option()
        {
            SEditorGUILayout.Vertical(
                SEditorGUILayout.Label("[Option]")
                + SEditorGUILayout.Horizontal(
                    SEditorGUILayout.Toggle("Gizmos Marching Cube", script.bVisibleMarchingCubeGizmos)
                        .OnValueChanged(value =>
                        {
                            script.bVisibleMarchingCubeGizmos = value;
                            SceneView.RepaintAll();
                        })
                    + SEditorGUILayout.Toggle("Gizmos Scalar Field", script.bVisibleScalarField)
                        .OnValueChanged(value =>
                        {
                            script.bVisibleScalarField = value;
                            SceneView.RepaintAll();
                        })
                )
            )
            .Style("box")
            .Render();
        }
    }

}
