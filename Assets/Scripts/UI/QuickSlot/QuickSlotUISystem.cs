using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;

namespace UI
{
    public class QuickSlotUISystem : SerializedMonoBehaviour, IUIComponent
    {
        public IItemContainer quickSlotContainer { get; private set; }

        public List<InventorySlotUIModel> slotUIModels;

        private void Awake()
        {
            slotUIModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<InventorySlotUIModel>())
                .Where(model => model != null)
                .ToList();
        }

        private void Start()
        {
            Render();
        }

        public void SetContainer(IItemContainer container)
        {
            quickSlotContainer = container;

            int slotCount = quickSlotContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                slotUIModels[i].SetItemStack(quickSlotContainer.GetItem(i));

            Render();
        }

        public void Render()
        {
            foreach (InventorySlotUIModel model in slotUIModels)
                model.Render();
        }
    }
}