using System;
using Corelib.SUI;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    [CustomEditor(typeof(BSP3DMapAsset))]
    public class EditorBSP3DMapAsset : Editor
    {
        private Vector3Int newSize;

        BSP3DMapAsset script;
        void OnEnable()
        {
            script = (BSP3DMapAsset)target;
            newSize = script.size;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Inspector_Size();

            Inspector_Param();

            SerializedProperty depthSplitCurveProp = serializedObject.FindProperty("depthSplitCurve");
            EditorGUILayout.PropertyField(depthSplitCurveProp, new GUIContent("Depth Split Curve"), true);

            SerializedProperty cellSizeCurveProp = serializedObject.FindProperty("cellSizeCurve");
            EditorGUILayout.PropertyField(cellSizeCurveProp, new GUIContent("Cell Size Curve"), true);

            serializedObject.ApplyModifiedProperties();
        }

        private void Inspector_Size()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Vector3Int("Size", newSize)
                    .OnValueChanged(value => newSize = value)
                + SEditorGUILayout.Button("Resize")
                    .OnClick(() => script.size = newSize)
            )
            .Render();
        }

        bool showParam = true;
        private void Inspector_Param()
        {
            var cellSizeContent = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Label("Cell Size")
                    + SEditorGUILayout.MinMaxSlider(script.minCellSize, script.maxCellSize)
                        .Range(0, 100)
                        .OnValueChanged((minValue, maxValue) =>
                        {
                            script.minCellSize = Mathf.RoundToInt(minValue);
                            script.maxCellSize = Mathf.RoundToInt(maxValue);
                        })
                )
                + SEditorGUILayout.Int("Min Cell Size", script.minCellSize)
                .OnValueChanged(value => script.minCellSize = value)
                + SEditorGUILayout.Int("Max Cell Size", script.maxCellSize)
                .OnValueChanged(value => script.maxCellSize = value)
            );

            var celSizeRatioContent = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Horizontal()
                .Content(

                )
            );

            var innerContent = SEditorGUILayout.FoldoutHeaderGroup("[Param]", showParam)
                .OnValueChanged(value => showParam = value)
                .Content(
                    SEditorGUILayout.Int("Max Depth", script.maxDepth)
                        .OnValueChanged(value =>
                        {
                            script.maxDepth = value;
                            script.InitliazeCurve();
                        })
                        .Clamp(1, 10)
                    + cellSizeContent
                    + celSizeRatioContent
                );

            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SEditorGUILayout.Horizontal("box")
                .Content(
                    SGUILayout.Space(15)
                    + SEditorGUILayout.Vertical()
                    .Content(
                        innerContent
                    )
                )
            )
            .Render();
        }
    }
}