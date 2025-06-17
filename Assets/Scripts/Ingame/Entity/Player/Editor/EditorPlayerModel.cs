using Corelib.SUI;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    [CustomEditor(typeof(PlayerModel), true)]
    public class EditorPlayerModel : EditorAgentModel
    {
        PlayerModel playerModel;
        protected void OnEnable()
        {
            base.OnEnable();
            playerModel = (PlayerModel)target;
        }

        bool isPlayerFold;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SEditorGUILayout.FoldGroup("Player Model", isPlayerFold)
            .OnValueChanged(value => isPlayerFold = value)
            .Content(
                RenderPlayerModel()
            )
            .Render();
        }

        private SUIElement RenderPlayerModel()
        {
            return SEditorGUILayout.Vertical();
        }
    }
}