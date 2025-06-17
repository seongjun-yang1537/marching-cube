using Corelib.SUI;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    [CustomEditor(typeof(AgentModel))]
    public partial class EditorAgentModel : InnerEditor<AgentModel>
    {
        private InnerGUI<AgentModel> guiBag;
        private InnerGUI<AgentModel> guiQuickSlot;
        private InnerGUI<AgentModel> guiEquipment;

        public void OnEnable()
        {
            base.OnEnable();

            guiBag = AddInnerGUI<BagGUI>();
            guiQuickSlot = AddInnerGUI<QuickSlotGUI>();
            guiEquipment = AddInnerGUI<EquipmentGUI>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Inspector_Inventory();
        }

        bool showInventory;
        bool showQuickSlot;
        bool showEquipment;

        private void Inspector_Inventory()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Label("[Inventory]")
                .Bold()
                .Align(TextAnchor.MiddleCenter)

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal()
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.FoldoutHeaderGroup("Inventory", showInventory)
                        .OnValueChanged(value => showInventory = value)
                    )
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showInventory) guiBag.OnInspectorGUI();
                    })
                )

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal()
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.FoldoutHeaderGroup("Quickslot", showQuickSlot)
                        .OnValueChanged(value => showQuickSlot = value)
                    )
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showQuickSlot) guiQuickSlot.OnInspectorGUI();
                    })
                )

                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal()
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.FoldoutHeaderGroup("Equipment", showEquipment)
                        .OnValueChanged(value => showEquipment = value)
                    )
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showEquipment) guiEquipment.OnInspectorGUI();
                    })
                )
            )
            .Render();
        }

        private void RenderInventory()
        {
            SEditorGUILayout.Vertical("box")
            .Content(

            )
            .Render();
        }

        private void RenderQuickSlot()
        {

        }

        private void RenderEquipment()
        {

        }
    }
}