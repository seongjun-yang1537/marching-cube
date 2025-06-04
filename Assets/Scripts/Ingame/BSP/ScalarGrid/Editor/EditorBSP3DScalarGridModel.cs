using Corelib.SUI;
using UnityEditor;

namespace Ingame
{
    [CustomEditor(typeof(BSP3DScalarGridModel))]
    public class EditorBSP3DScalarGridModel : Editor
    {
        private Editor BSP3DScalarGridEditor;
        BSP3DScalarGridModel script;
        void OnEnable()
        {
            script = (BSP3DScalarGridModel)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();
            Insepctor_BSP3DScalarGridScriptableObject();

            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Button("Generate")
                .OnClick(() =>
                {
                    SceneView.RepaintAll();
                })
            )
            .Render();

            serializedObject.ApplyModifiedProperties();
        }

        bool showBSP3DScalarGridScriptableObject = true;
        private void Insepctor_BSP3DScalarGridScriptableObject()
        {
            // if (!script.scalarGrid)
            //     return;

            SEditorGUILayout.Vertical("helpbox")
            .Content(
                SGUILayout.Horizontal(
                    SGUILayout.Space(15)
                    + SEditorGUILayout.FoldoutHeaderGroup("BSP3D Scalar Field Inspector", showBSP3DScalarGridScriptableObject)
                    .OnValueChanged(value => showBSP3DScalarGridScriptableObject = value)
                )
            )
            .Render();

            if (!showBSP3DScalarGridScriptableObject)
                return;

            // SEditorGUILayout.Vertical("helpbox")
            // .Content(
            //     SEditorGUILayout.Action(() =>
            //     {
            //         if (BSP3DScalarGridEditor == null || BSP3DScalarGridEditor.target != script.scalarGrid)
            //         {
            //             BSP3DScalarGridEditor = CreateEditor(script.scalarGrid);
            //         }
            //         BSP3DScalarGridEditor.OnInspectorGUI();
            //     })
            // )
            // .Render();
        }
    }
}