using UnityEngine;
using UnityEditor;
using Corelib.SUI;
using Unity.VisualScripting;

namespace Ingame
{
    [CustomEditor(typeof(EntityModel), true)]
    public class EditorEntityModel : Editor
    {
        EntityModel entityModel;
        protected void OnEnable()
        {
            entityModel = (EntityModel)target;
        }

        bool isEntityFold = true;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SEditorGUILayout.FoldGroup("Entity Model", isEntityFold)
            .OnValueChanged(value => isEntityFold = value)
            .Content(
                RenderEntityModel()
            )
            .Render();

            serializedObject.ApplyModifiedProperties();
        }

        private SUIElement RenderEntityModel()
        {
            return SEditorGUILayout.Vertical()
            .Content(
                SEditorGUILayout.Float($"Life ({entityModel.lifeRatio.ToString("F2")})%", entityModel.life)
                .OnValueChanged(value => entityModel.SetLife(value))
                + SEditorGUILayout.Float("Max Life", entityModel.lifeMax)
                .OnValueChanged(value => entityModel.SetLifeMax(value))
            );
        }
    }
}