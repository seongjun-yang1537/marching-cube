using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;

namespace UI
{
    public class InventorySlotUISystem : SerializedMonoBehaviour, IUIComponent
    {
        public IItemContainer inventoryContainer { get; private set; }

        public List<InventorySlotUIModel> slotUIModels;

        private void Awake()
        {
            slotUIModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<InventorySlotUIModel>())
                .Where(model => model != null)
                .ToList();
        }

        public void SetContainer(IItemContainer container)
        {
            inventoryContainer = container;

            int slotCount = inventoryContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                slotUIModels[i].SetItemStack(inventoryContainer.GetItem(i));

            Render();
        }

        public void Render()
        {
            foreach (InventorySlotUIModel model in slotUIModels)
                model.Render();
        }
    }
}