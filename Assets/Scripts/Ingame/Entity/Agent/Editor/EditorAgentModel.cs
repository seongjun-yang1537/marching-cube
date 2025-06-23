using Corelib.SUI;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    [CustomEditor(typeof(AgentModel), true)]
    public partial class EditorAgentModel : EditorEntityModel
    {
        AgentModel agentModel;

        private InnerGUI<AgentModel> guiBag;
        private InnerGUI<AgentModel> guiQuickSlot;
        private InnerGUI<AgentModel> guiEquipment;

        protected void OnEnable()
        {
            base.OnEnable();

            agentModel = (AgentModel)target;

            guiBag = InnerEditor<AgentModel>.CreateInnerGUI<BagGUI>(agentModel);
            guiQuickSlot = InnerEditor<AgentModel>.CreateInnerGUI<QuickSlotGUI>(agentModel);
            guiEquipment = InnerEditor<AgentModel>.CreateInnerGUI<EquipmentGUI>(agentModel);
        }

        bool isFoldAgent = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SEditorGUILayout.FoldGroup("Agent Model", isFoldAgent)
            .OnValueChanged(value => isFoldAgent = value)
            .Content(
                RenderAgentModel()
            )
            .Render();
        }

        private SUIElement RenderAgentModel()
        {
            return SEditorGUILayout.Vertical()
                .Content(
                    RenderHeldItem()
                    + RenderInventory()
                );
        }

        private SUIElement RenderHeldItem()
        {
            return SEditorGUILayout.Vertical()
            .Content(

            );
        }

        bool showInventory;
        bool showQuickSlot;
        bool showEquipment;

        private SUIElement RenderInventory()
        {
            return SEditorGUILayout.Group("Inventory")
            .Content(
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal()
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.FoldoutHeaderGroup("Inventory", showInventory)
                        .OnValueChanged(value => showInventory = value)
                    )
                    + SEditorGUILayout.Action(() =>
                    {
                        if (showInventory) guiBag?.OnInspectorGUI();
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
                        if (showQuickSlot) guiQuickSlot?.OnInspectorGUI();
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
                        if (showEquipment) guiEquipment?.OnInspectorGUI();
                    })
                )
            );
        }
    }
}