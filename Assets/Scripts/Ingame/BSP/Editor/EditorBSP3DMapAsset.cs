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

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
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
        bool showSplitByDepthCurve = false;
        bool showRoomSizeRatioCurve = false;
        bool showSplitLengthRatioCurve = false;
        bool showRoomByDepthCurve = false;
        private void Inspector_Param()
        {
            BSP3DGenerationParam param = script.param;

            var cellSizeContent = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Cell Size]")
                .Bold()
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSize.x >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSize.x = value ? 0 : -1;
                        param.maxCellSize.x = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("X")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Int("", param.minCellSize.x)
                    .OnValueChanged(value => param.minCellSize.x = value)
                    .Width(64)
                    .Clamp(0, script.size.x)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSize.x, param.maxCellSize.x)
                    .Range(0, script.size.x)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSize.x = Mathf.RoundToInt(minValue);
                        param.maxCellSize.x = Mathf.RoundToInt(maxValue);
                    })
                    .Width(256)

                    + SEditorGUILayout.Int("", param.maxCellSize.x)
                    .OnValueChanged(value => param.maxCellSize.x = value)
                    .Width(64)
                    .Clamp(-1, script.size.x)
                )
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSize.y >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSize.y = value ? 0 : -1;
                        param.maxCellSize.y = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("Y")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Int("", param.minCellSize.y)
                    .OnValueChanged(value => param.minCellSize.y = value)
                    .Width(64)
                    .Clamp(0, script.size.y)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSize.y, param.maxCellSize.y)
                    .Range(0, script.size.y)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSize.y = Mathf.RoundToInt(minValue);
                        param.maxCellSize.y = Mathf.RoundToInt(maxValue);
                    })
                    .Width(256)

                    + SEditorGUILayout.Int("", param.maxCellSize.y)
                    .OnValueChanged(value => param.maxCellSize.y = value)
                    .Width(64)
                    .Clamp(-1, script.size.y)
                )
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSize.z >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSize.z = value ? 0 : -1;
                        param.maxCellSize.z = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("Z")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Int("", param.minCellSize.z)
                    .OnValueChanged(value => param.minCellSize.z = value)
                    .Width(64)
                    .Clamp(0, script.size.z)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSize.z, param.maxCellSize.z)
                    .Range(0, script.size.z)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSize.z = Mathf.RoundToInt(minValue);
                        param.maxCellSize.z = Mathf.RoundToInt(maxValue);
                    })
                    .Width(256)

                    + SEditorGUILayout.Int("", param.maxCellSize.z)
                    .OnValueChanged(value => param.maxCellSize.z = value)
                    .Width(64)
                    .Clamp(-1, script.size.z)
                )
            );

            var cellSizeRatioContent = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Cell Size Ratio]")
                .Bold()
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSizeRatio.x >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSizeRatio.x = value ? 0 : -1;
                        param.maxCellSizeRatio.x = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("X")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Float("", param.minCellSizeRatio.x)
                    .OnValueChanged(value => param.minCellSizeRatio.x = value)
                    .Width(64)
                    .Clamp(0, 1)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSizeRatio.x, param.maxCellSizeRatio.x)
                    .Range(0, 1)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSizeRatio.x = minValue;
                        param.maxCellSizeRatio.x = maxValue;
                    })
                    .Width(256)

                    + SEditorGUILayout.Float("", param.maxCellSizeRatio.x)
                    .OnValueChanged(value => param.maxCellSizeRatio.x = value)
                    .Width(64)
                    .Clamp(-1, 1)
                )
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSizeRatio.y >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSizeRatio.y = value ? 0 : -1;
                        param.maxCellSizeRatio.y = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("Y")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Float("", param.minCellSizeRatio.y)
                    .OnValueChanged(value => param.minCellSizeRatio.y = value)
                    .Width(64)
                    .Clamp(0, 1)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSizeRatio.y, param.maxCellSizeRatio.y)
                    .Range(0, 1)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSizeRatio.y = minValue;
                        param.maxCellSizeRatio.y = maxValue;
                    })
                    .Width(256)

                    + SEditorGUILayout.Float("", param.maxCellSizeRatio.y)
                    .OnValueChanged(value => param.maxCellSizeRatio.y = value)
                    .Width(64)
                    .Clamp(-1, 1)
                )
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", param.maxCellSizeRatio.z >= 0)
                    .OnValueChanged(value =>
                    {
                        param.minCellSizeRatio.z = value ? 0 : -1;
                        param.maxCellSizeRatio.z = value ? 0 : -1;
                    })
                    .Width(16)

                    + SEditorGUILayout.Label("Z")
                    .Width(16)

                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Float("", param.minCellSizeRatio.z)
                    .OnValueChanged(value => param.minCellSizeRatio.z = value)
                    .Width(64)
                    .Clamp(0, 1)

                    + SEditorGUILayout.MinMaxSlider(param.minCellSizeRatio.z, param.maxCellSizeRatio.z)
                    .Range(0, 1)
                    .OnValueChanged((minValue, maxValue) =>
                    {
                        param.minCellSizeRatio.z = minValue;
                        param.maxCellSizeRatio.z = maxValue;
                    })
                    .Width(256)

                    + SEditorGUILayout.Float("", param.maxCellSizeRatio.z)
                    .OnValueChanged(value => param.maxCellSizeRatio.z = value)
                    .Width(64)
                    .Clamp(-1, 1)
                )
            );

            var curveContent = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal("helpbox")
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.Foldout("SplitByDepthCurve", showSplitByDepthCurve)
                        .OnValueChanged(value => showSplitByDepthCurve = value)
                    )
                    + SGUILayout.Space(10)
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showSplitByDepthCurve)
                            BSP3DProbabilityCurveDrawer.Draw(param.splitByDepthCurve);
                    })
                )

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal("helpbox")
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.Foldout("RoomSizeRatioCurve", showRoomSizeRatioCurve)
                        .OnValueChanged(value => showRoomSizeRatioCurve = value)
                    )
                    + SGUILayout.Space(10)
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showRoomSizeRatioCurve)
                            BSP3DProbabilityCurveDrawer.DrawVector3(param.roomSizeRatioCurve);
                    })
                )

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal("helpbox")
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.Foldout("SplitLengthRatioCurve", showSplitLengthRatioCurve)
                        .OnValueChanged(value => showSplitLengthRatioCurve = value)
                    )
                    + SGUILayout.Space(10)
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showSplitLengthRatioCurve)
                            BSP3DProbabilityCurveDrawer.DrawVector3(param.splitLengthRatioCurve);
                    })
                )

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal("helpbox")
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.Foldout("RoomByDepthCurve", showRoomByDepthCurve)
                        .OnValueChanged(value => showRoomByDepthCurve = value)
                    )
                    + SGUILayout.Space(10)
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showRoomByDepthCurve)
                            BSP3DProbabilityCurveDrawer.Draw(param.roomByDepthCurve);
                    })
                )
            );

            var innerContent = SEditorGUILayout.FoldoutHeaderGroup("[Param]", showParam)
            .OnValueChanged(value => showParam = value)
            .Content(
                SEditorGUILayout.Int("Max Depth", param.maxDepth)
                    .OnValueChanged(value => param.maxDepth = value)
                    .Clamp(1, 10)
                + cellSizeContent
                + cellSizeRatioContent
                + curveContent
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