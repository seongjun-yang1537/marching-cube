using UnityEngine;
using UnityEditor;
using Corelib.SUI;
using Ingame;

namespace UI
{
    [CustomEditor(typeof(InventorySlotUIModel))]
    public class EditorInventorySlotUIModel : Editor
    {
        InventorySlotUIModel script;
        void OnEnable()
        {
            script = (InventorySlotUIModel)target;
        }

        ItemID selectedItemID;
        int itemCount;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.EnumPopup("dd", selectedItemID);

            SEditorGUILayout.Vertical("Box")
            .Content(
                SEditorGUILayout.Label("[Debug]")
                .Bold()
                .Align(TextAnchor.MiddleCenter)
                + SEditorGUILayout.Enum("Item ID", selectedItemID)
                .OnValueChanged(value => selectedItemID = (ItemID)value)
                + SEditorGUILayout.Int("Item Count", itemCount)
                .OnValueChanged(value => itemCount = value)
                + SEditorGUILayout.Horizontal()
                .Content(
                    SEditorGUILayout.Button("Render")
                    .OnClick(() =>
                    {
                        script.SetItemStack(new ItemStack(new ItemData(selectedItemID), itemCount));
                        SceneView.RepaintAll();
                    })
                    + SEditorGUILayout.Button("Remove")
                    .OnClick(() =>
                    {
                        script.RemoveItemStack();
                        SceneView.RepaintAll();
                    })
                )
            )
            .Render();
        }
    }
}