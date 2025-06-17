using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Ingame;
using System;

namespace UI
{
    public class UIBagSlots : UIComponentBahaviour
    {
        public BagContainer bagContainer { get; private set; }

        public List<UIBagSlotModel> uiSlotModels;

        protected void Awake()
        {
            base.Awake();

            uiSlotModels = transform.Cast<Transform>()
                .Select(t => t.GetComponent<UIBagSlotModel>())
                .Where(model => model != null)
                .ToList();
        }

        public void SetContainer(BagContainer container)
        {
            bagContainer = container;

            int slotCount = bagContainer.SlotCount;
            for (int i = 0; i < slotCount; i++)
                uiSlotModels[i].SetItemSlot(bagContainer.GetSlot((BagSlotID)i));

            Render();
        }

        public override void Render()
        {
            foreach (UIBagSlotModel model in uiSlotModels)
                model.Render();
        }
    }
}