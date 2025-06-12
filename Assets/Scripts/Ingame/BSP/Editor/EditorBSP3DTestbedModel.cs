using Corelib.SUI;
using Corelib.Utils;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

namespace Ingame
{
    [CustomEditor(typeof(BSP3DTestbedModel))]
    public class EditorBSP3DTestbedModel : Editor
    {
        BSP3DTestbedModel script;
        private void OnEnable()
        {
            script = (BSP3DTestbedModel)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();
            SEditorGUILayout.Vertical("Box")
            .Content(
                SEditorGUILayout.Vector3Int("Size", script.Size)
                .OnValueChanged(value => script.Size = value)
                + SEditorGUILayout.Button("Generate")
                .OnClick(() =>
                {
                    {
                        BSP3DGenerationContext context =
                            new BSP3DGenerationContext.Builder(MT19937.Create(), script.playModel)
                            .Preset(BSP3DGenerationPreset.CAVE)
                            .Build();

                        EditorCoroutineUtility.StartCoroutineOwnerless(
                            script.playModel.GenerateAsync(context)
                        );
                    }
                    {
                        BSP3DGenerationContext context =
                            new BSP3DGenerationContext.Builder(MT19937.Create(), script.testbedModel)
                            .Preset(BSP3DGenerationPreset.TESTBED)
                            .Build();

                        EditorCoroutineUtility.StartCoroutineOwnerless(
                            script.testbedModel.GenerateAsync(context)
                        );
                    }
                })
            )
            .Render();

            serializedObject.ApplyModifiedProperties();
        }
    }
}