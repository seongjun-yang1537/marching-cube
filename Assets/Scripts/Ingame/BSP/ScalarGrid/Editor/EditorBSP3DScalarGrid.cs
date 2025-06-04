// using Corelib.SUI;
// using MCube;
// using UnityEditor;

// namespace Ingame
// {
//     [CustomEditor(typeof(BSP3DScalarGrid))]
//     public class EditorBSP3DScalarGrid : Editor
//     {
//         BSP3DScalarGrid script;
//         void OnEnable()
//         {
//             script = (BSP3DScalarGrid)target;
//         }

//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();

//             SEditorGUILayout.Vertical()
//             .Content(
//                 SEditorGUILayout.Button("gg")
//                 .OnClick(() =>
//                 {
//                     ScalarField scalarField = script.CreateAllScalarField();
//                     AssetDatabase.CreateAsset(scalarField, "Assets/MyGeneratedScalarField.asset");
//                     AssetDatabase.SaveAssets();
//                     AssetDatabase.Refresh();
//                 })
//             )
//             .Render();
//         }
//     }
// }