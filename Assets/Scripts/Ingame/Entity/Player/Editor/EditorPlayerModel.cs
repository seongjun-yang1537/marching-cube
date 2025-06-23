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

        bool isPlayerFold = true;
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
            return SEditorGUILayout.Vertical()
            .Content(
                SEditorGUILayout.Float($"Stemina ({playerModel.steminaRatio.ToString("F2")})%", playerModel.stemina)
                .OnValueChanged(value => playerModel.SetStemina(value))
                + SEditorGUILayout.Float("Max Stemina", playerModel.steminaMax)
                .OnValueChanged(value => playerModel.SetSteminaMax(value))
                + SEditorGUILayout.Float($"Fuel ({playerModel.fuelRatio.ToString("F2")})%", playerModel.fuel)
                .OnValueChanged(value => playerModel.SetStemina(value))
                + SEditorGUILayout.Float("Max Fuel", playerModel.fuelMax)
                .OnValueChanged(value => playerModel.SetSteminaMax(value))
            );
        }
    }
}