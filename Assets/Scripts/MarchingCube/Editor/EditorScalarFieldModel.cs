using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            { EditorGUILayout.BeginVertical("box"); }
            {
                script.bVisibleMarchingCubeGizmos = EditorGUILayout.Toggle(script.bVisibleMarchingCubeGizmos, "Marching Cube Gizmos");
                script.bVisibleScalarField = EditorGUILayout.Toggle(script.bVisibleScalarField, "Scalar Field");
            }
            { EditorGUILayout.EndVertical(); }
        }
    }

}
