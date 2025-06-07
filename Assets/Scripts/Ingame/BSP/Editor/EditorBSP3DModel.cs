using Corelib.SUI;
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

            Inspector_BSP3DMapAssetScriptableObject();

            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Button("Generate")
                .OnClick(() =>
                {
                    script.Generate();
                    SceneView.RepaintAll();
                })
            )
            .Render();

            Inspector_GizmosOption();

            serializedObject.ApplyModifiedProperties();
        }

        bool showBSP3DMapAssetScriptableObject = true;
        private void Inspector_BSP3DMapAssetScriptableObject()
        {
            if (!script.config)
                return;

            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SGUILayout.Horizontal(
                    SGUILayout.Space(15)
                    + SEditorGUILayout.FoldoutHeaderGroup("BSP3D Config Inspector", showBSP3DMapAssetScriptableObject)
                    .OnValueChanged(value => showBSP3DMapAssetScriptableObject = value)
                )
            )
            .Render();

            if (showBSP3DMapAssetScriptableObject)
            {
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Action(() =>
                    {
                        if (BSP3DMapAssetEditor == null || BSP3DMapAssetEditor.target != script.config)
                        {
                            BSP3DMapAssetEditor = CreateEditor(script.config);
                        }
                        BSP3DMapAssetEditor.OnInspectorGUI();
                    })
                )
                .Render();
            }
        }

        private void Inspector_GizmosOption()
        {
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
            )
            .Render();
        }
    }
}