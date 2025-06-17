using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;

namespace UI
{
    public class UIQuickSlots : UIComponentBahaviour
    {
        public QuickSlotContainer quickSlotContainer { get; private set; }

        public List<UIQuickSlotModel> uiSlotModels;

        protected void Awake()
        {
            base.Awake();

            uiSlotModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<UIQuickSlotModel>())
                .Where(model => model != null)
                .ToList();
        }

        private void Start()
        {
            Render();
        }

        public void SetContainer(QuickSlotContainer container)
        {
            quickSlotContainer = container;

            int slotCount = quickSlotContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                uiSlotModels[i].SetItemSlot(quickSlotContainer.GetSlot((QuickSlotID)i));

            Render();
        }

        public override void Render()
        {
            foreach (UIQuickSlotModel model in uiSlotModels)
                model.Render();
        }
    }
}