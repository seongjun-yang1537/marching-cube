using System;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer.Lock;
using Corelib.SUI;
using Corelib.Utils;
using UnityEditor;
using UnityEngine;
namespace Ingame
{
    public partial class EditorAgentModel : InnerEditor<AgentModel>
    {
        private class InventoryGUI : InnerGUI<AgentModel>
        {
            private Inventory inventory { get => script.inventory; }

            int selectedIdx;

            ItemID itemID;
            int itemCount;

            public override void OnInspectorGUI()
            {
                SEditorGUILayout.Vertical("box")
                .Content(
                    RenderInventoryGrid()
                    + RenderController()
                )
                .Render();
            }

            private SUIElement RenderInventoryGrid()
            {
                List<GUIContent> guiContents = CreateItemStackGUIContents();

                return SEditorGUILayout.Vertical("box")
                .Content(
                    SGUILayout.SelectionGrid(selectedIdx, guiContents, 5)
                    .OnValueChanged(value => selectedIdx = value)
                );
            }

            private SUIElement RenderController()
            {
                return SEditorGUILayout.Vertical("box")
                .Content(
                    SEditorGUILayout.Label("[Controller]")
                    .Bold()
                    + SEditorGUILayout.Enum("Item ID", itemID)
                    .OnValueChanged(value => itemID = (ItemID)value)
                    + SEditorGUILayout.Int("Count", itemCount)
                    .OnValueChanged(value => itemCount = value)
                    + SEditorGUILayout.Horizontal()
                    .Content(
                        SEditorGUILayout.Button("Apply")
                        .OnClick(() =>
                        {
                            if (itemCount > 0)
                                inventory.SetItemSlot(
                                    (InventorySlotID)selectedIdx,
                                    new ItemStack(itemID, itemCount)
                                );
                        })
                        + SEditorGUILayout.Button("Remove")
                        .OnClick(() =>
                        {
                            inventory.SetItemSlot((InventorySlotID)selectedIdx, ItemData.Empty());
                        })
                    )
                );
            }

            private GUIContent CreateItemStackGUIContent(ItemStack itemStack)
            {
                if (itemStack == null || itemStack.IsEmpty)
                    return new GUIContent();

                Texture2D icon = ItemDB.GetEditorIconTexture(itemStack.itemID);
                string label = itemStack.count > 1 ? $"x{itemStack.count}" : "";
                string tooltip = "";

                return new GUIContent(icon, $"{tooltip} {label}");
            }

            private List<GUIContent> CreateItemStackGUIContents()
            {
                List<GUIContent> contents = new();
                foreach (InventorySlotID slotID in Enum.GetValues(typeof(InventorySlotID)))
                    contents.Add(CreateItemStackGUIContent(inventory.GetItemSlot(slotID)));
                return contents;
            }
        }
    }
}