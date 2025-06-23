using System;
using Corelib.SUI;
using Corelib.Utils;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Ingame
{
    [CustomEditor(typeof(BSP3DModel))]
    public class EditorBSP3DModel : Editor
    {
        private Editor BSP3DMapAssetEditor;
        BSP3DModel script;
        void OnEnable()
        {
            script = (BSP3DModel)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Inspector_MapModel();
            Inspector_BSP3DMapAssetScriptableObject();
            Inspector_Generator();
            Inspector_GizmosOption();

            serializedObject.ApplyModifiedProperties();
        }

        private void Inspector_MapModel()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Object("Map Asset", script.mapAsset, typeof(BSP3DMapAsset))
                .OnValueChanged(value => script.mapAsset = value as BSP3DMapAsset)
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Toggle("", script.seed != -1)
                    .OnValueChanged(value => script.seed = value ? 0 : -1)
                    .Width(16)
                    + SEditorGUILayout.Label("Seed")
                    .Width(32)
                    + SGUILayout.FlexibleSpace()
                    + SEditorGUILayout.Int("", script.seed)
                    .OnValueChanged(value => script.seed = value)
                )
            )
            .Render();
        }

        bool showBSP3DMapAssetScriptableObject = true;
        private void Inspector_BSP3DMapAssetScriptableObject()
        {
            if (!script.mapAsset)
                return;

            SEditorGUILayout.FoldGroup("BSP3D MapAsset Inspector", showBSP3DMapAssetScriptableObject)
            .OnValueChanged(value => showBSP3DMapAssetScriptableObject = value)
            .Content(
                SEditorGUILayout.Action(() =>
                {
                    if (BSP3DMapAssetEditor == null || BSP3DMapAssetEditor.target != script.mapAsset)
                    {
                        BSP3DMapAssetEditor = CreateEditor(script.mapAsset);
                    }
                    BSP3DMapAssetEditor.OnInspectorGUI();
                })
            )
            .Render();
        }

        private void Inspector_GizmosOption()
        {
            var projectionGrid = SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Room Projection Grid]")
                .Bold()
                + SEditorGUILayout.Action(() =>
                {
                    foreach (BSP3DCubeFace face in Enum.GetValues(typeof(BSP3DCubeFace)))
                    {
                        SEditorGUILayout.Toggle(face.ToString(), script.gizmosOption.VisibleRoomProjectionGrid[face])
                        .OnValueChanged(value =>
                        {
                            script.gizmosOption.VisibleRoomProjectionGrid[face] = value;
                            SceneView.RepaintAll();
                        })
                        .Render();
                    }
                })
            );

            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Gizmos Option]")
                    .Bold()
                    .Align(TextAnchor.MiddleCenter)
                + SEditorGUILayout.Toggle("Visible BSP Area", script.gizmosOption.visibleBSPArea)
                    .OnValueChanged(value =>
                    {
                        script.gizmosOption.visibleBSPArea = value;
                        SceneView.RepaintAll();
                    })
                + SEditorGUILayout.Toggle("Visible Room", script.gizmosOption.visibleRoom)
                    .OnValueChanged(value =>
                    {
                        script.gizmosOption.visibleRoom = value;
                        SceneView.RepaintAll();
                    })
                + SEditorGUILayout.Toggle("Visible Room Graph", script.gizmosOption.visibleRoomGraph)
                    .OnValueChanged(value =>
                    {
                        script.gizmosOption.visibleRoomGraph = value;
                        SceneView.RepaintAll();
                    })
                + projectionGrid
            )
            .Render();
        }

        float progressGeneration;
        private void Inspector_Generator()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.ProgressBar("Generation", progressGeneration)
                + SEditorGUILayout.Button("Generate")
                .OnClick(() =>
                {
                    MT19937 rng = script.seed == -1 ? MT19937.Create() : MT19937.Create(script.seed);

                    BSP3DGenerationContext context =
                        new BSP3DGenerationContext.Builder(rng, script)
                        .Preset(BSP3DGenerationPreset.CAVE)
                        .ProgressCallback(progress => progressGeneration = progress)
                        .Build();

                    EditorCoroutineUtility.StartCoroutineOwnerless(
                        script.GenerateAsync(context)
                    );
                    SceneView.RepaintAll();
                })
            )
            .Render();
        }
    }
}