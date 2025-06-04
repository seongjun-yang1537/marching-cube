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
            Inspector_Generator();
            Inspector_Option();

            serializedObject.ApplyModifiedProperties();
        }

        bool showScalarFieldScriptableObject = true;
        private void Inspector_ScalarFieldScriptableObject()
        {
            if (!script.scalarField)
                return;

            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SGUILayout.Horizontal(
                    SGUILayout.Space(15)
                    + SEditorGUILayout.FoldoutHeaderGroup("Scalar Field Inspector", showScalarFieldScriptableObject)
                        .OnValueChanged(value => showScalarFieldScriptableObject = value)
                )
            )
            .Render();

            if (showScalarFieldScriptableObject)
            {
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Action(() =>
                    {
                        if (scalarFieldEditor == null || scalarFieldEditor.target != script.scalarField)
                        {
                            scalarFieldEditor = CreateEditor(script.scalarField);
                        }

                        scalarFieldEditor.OnInspectorGUI();
                    })
                )
                .Render();
            }

            { EditorGUILayout.EndFoldoutHeaderGroup(); }
        }

        private void Inspector_Option()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Option]")
                + SEditorGUILayout.Horizontal()
                    .Content(
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
            .Render();
        }

        private void Inspector_Generator()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Generator]")
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Button("Random")
                    .OnClick(() =>
                    {
                        ScalarFieldGenerator.Random(script.scalarField);
                        script.GenerateMarchingCubeMesh();
                    })
                    + SEditorGUILayout.Button("Periln Noise")
                    .OnClick(() =>
                    {
                        ScalarFieldGenerator.PerlinNoise(script.scalarField);
                        script.GenerateMarchingCubeMesh();
                    })
                )
                + SEditorGUILayout.Button("Generate Marching Cube")
                .OnClick(() => script.GenerateMarchingCube())
            )
            .Render();
        }
    }

}
