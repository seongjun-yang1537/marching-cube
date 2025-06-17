using System.Collections.Generic;
using System.Linq;
using Corelib.Utils;
using Ingame;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class InventorySelectionUISystem : UIComponentBahaviour
    {
        private Inventory inventory { get => PlayerManager.Instance.PlayerModel.inventory; }

        [VerticalGroup("UI System")]
        public BagUISystem bagUISystem;
        [VerticalGroup("UI System")]
        public QuickSlotUISystem quickSlotUISystem;
        [VerticalGroup("UI System")]
        public EquipmentUISystem equipmentUISystem;

        public UIDraggingItem uiDraggingItem { get => GetChildUI<UIDraggingItem>(); }
        public Transform selectionUI;

        private UIInventorySelection uiSelection;

        private List<UIItemSlot> uiItemSlots;

        protected void Start()
        {
            InitializeSlots();
            uiSelection = GetChildUI<UIInventorySelection>();
        }

        private void InitializeSlots()
        {
            uiItemSlots = new();
            uiItemSlots.AddRange(bagUISystem.FindAllChild<UIItemSlot>());
            uiItemSlots.AddRange(quickSlotUISystem.FindAllChild<UIItemSlot>());
            uiItemSlots.AddRange(equipmentUISystem.FindAllChild<UIItemSlot>());

            foreach (var slot in uiItemSlots)
            {
                slot.onClick.AddListener(() => SelectSlot(slot));
                slot.onDragStart.AddListener(eventData => OnDragStart(eventData));
                slot.onDrag.AddListener(eventData => OnDrag(eventData));
                slot.onDragEnd.AddListener(eventData => OnDragEnd(eventData));
            }
        }

        private UIItemSlot GetItemSlotByEventData(PointerEventData eventData)
        {
            List<RaycastResult> raycastResults = new();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            return raycastResults
                .Select(result => result.gameObject.GetComponent<UIItemSlot>())
                .FirstOrDefault(slot => slot != null);
        }

        private ItemSlot selectedItemSlot;
        private void OnDragStart(PointerEventData eventData)
        {
            UIItemSlot uiItemSlot = GetItemSlotByEventData(eventData);
            if (uiItemSlot == null) return;

            selectedItemSlot = new ItemSlot(uiItemSlot.itemSlot);
            uiDraggingItem.SetItemStack(selectedItemSlot.itemStack);
        }

        private void OnDrag(PointerEventData eventData)
        {
            uiDraggingItem.transform.position = eventData.position;
        }

        private void OnDragEnd(PointerEventData eventData)
        {
            if (selectedItemSlot != null)
            {
                UIItemSlot uiItemSlot = GetItemSlotByEventData(eventData);
                if (uiItemSlot == null) return;
                inventory.SwapItemSlot(selectedItemSlot, uiItemSlot.itemSlot);
                SelectSlot(uiItemSlot);
                uiDraggingItem.SetItemStack(null);
            }
        }

        private void SelectSlot(UIItemSlot slot)
        {
            uiSelection.nowSelectSlot = slot;
            Render();
        }

        public override void Render()
        {
            uiSelection.Render();
        }
    }
}