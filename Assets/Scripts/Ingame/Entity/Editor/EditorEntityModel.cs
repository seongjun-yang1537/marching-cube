using UnityEngine;
using UnityEditor;
using Corelib.SUI;

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

        bool isEntityFold;
        public override void OnInspectorGUI()
        {
            SEditorGUILayout.FoldGroup("Entity Model", isEntityFold)
            .OnValueChanged(value => isEntityFold = value)
            .Content(
                RenderEntityModel()
            )
            .Render();
        }

        private SUIElement RenderEntityModel()
        {
            return SEditorGUILayout.Vertical()
            .Content(SEditorGUILayout.Label("hello"));
        }
    }
}